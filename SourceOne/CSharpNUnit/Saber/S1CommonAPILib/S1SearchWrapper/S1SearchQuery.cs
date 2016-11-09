using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Saber.S1CommonAPILib.S1SearchWrapper
{
    /// <summary>
    /// The Search Field Control Item type enumeration
    /// </summary>
    public enum FieldItemType
    {
        Address,
        DateTime,
        String,
        BooleanType,
        ComboType,
        NumberType,
        Removed
    }

    public enum FieldDataType
    {
        String = 0,
        DateTime = 1,
        Int32 = 2,
        Int64 = 3,
        Boolean = 4,
        Undefined = 5,
    }
    /// <summary>
    /// The interface that describes the search field control behavior and allows access to the classes in the app_code folder
    /// </summary>
    public interface ISearchFieldControl
    {
        FieldItemType FieldType { get; set; }

        string FieldTag { get; set; }

        string FieldName { get; set; }

        string XmlSearchCriteria { get; set; }

        void GetStringValue(out String s);

        void SetStringValue(String sValue);

        void GetDateTimeValue(out String sSpecifier, out DateTime dtDate);

        void SetDateTimeValue(String sSpecifier, DateTime dtDate);

        void GetNumberValue(out String sSpecifier, out String sNumber);

        void SetNumberValue(String sSpecifier, String sNumber);

        void GetAddressValue(out List<KeyValuePair<string, ArrayList>> listAddrsAuto, out String sAddrsMan);

        void SetAddressValue(List<KeyValuePair<string, ArrayList>> listAddrsAuto, String sAddrsMan);

        void GetBooleanValue(out String sValue);

        void SetBooleanValue(String sValue);

        void GetComboValue(out String sValue);

        void SetComboValue(String sValue);

        void SetDisplayAndTypeFromTag(String FieldTag);

        void SyncHiddenData();
    }

    /// <summary>
    /// QueryBuilder builds query Xml from search fields
    /// </summary>
    public class SearchQueryBuilder
    {
        private StringBuilder sbQuery;
        private XmlWriter xWriter;
        private XmlWriterSettings xWriteSettings;
        private bool bQueryHasData;
        private bool bAllowEmptyQuery;
        private string m_sError;
        private string m_sErrorDetail;
        private SearchPresentation sp;
        private string m_EmailEnv;

        public SearchQueryBuilder()
        {
            //ExSearchTrace trace = new ExSearchTrace();
            sp = new SearchPresentation();
            bAllowEmptyQuery = true;
        }

        public SearchQueryBuilder(bool allowEmptyQuery)
        {
            //ExSearchTrace trace = new ExSearchTrace();
            sp = new SearchPresentation();
            bAllowEmptyQuery = allowEmptyQuery;
        }

        ///// <summary>
        ///// Load XML from activity db
        ///// </summary>
        ///// <param name="name">[in] name of xml doc</param>
        ///// <returns>XML data document</returns>
        //private XmlDataDocument GetXmlFromDB(string name)
        //{
        //    ExSearchTrace trace = new ExSearchTrace();
        //    XmlDataDocument xmlDoc = new XmlDataDocument();

        //    try
        //    {
        //        CoExJDFAPIMgr mgr = new CoExJDFAPIMgr();
        //        IExObjectSchemaMapCollection c = (IExObjectSchemaMapCollection)mgr.GetExObjectSchemaMap();

        //        CoExObjectSchemaMap m = c.GetSchemaMapByName(name);
        //        xmlDoc.LoadXml(m.Serialize());
        //    }
        //    catch (Exception e)
        //    {
        //        trace.TraceError(e.ToString());
        //        throw e;
        //    }

        //    return xmlDoc;
        //}


        public string CreateXMLQuery(String sKeywordValue, List<ISearchFieldControl> listFields, String sObjectId, String sSearchType, String sMailEnv)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            m_sError = String.Empty;
            m_sErrorDetail = String.Empty;
            sbQuery = new StringBuilder();
            xWriteSettings = new XmlWriterSettings();
            xWriteSettings.Indent = true;
            xWriter = XmlWriter.Create(sbQuery, xWriteSettings);
            bQueryHasData = false;
            bool bSaveQueryHasData = false;
            m_EmailEnv = sMailEnv;

            try
            {
                //****************************************
                //Generate ExpressionSet Nodes
                //****************************************
                xWriter.WriteStartElement(QueryXmlDefines.EXPRESSIONSET);
                //fields are AND'd together
                xWriter.WriteAttributeString(QueryXmlDefines.LOGICALOPERATOR, QueryXmlDefines.AND);

                //Add keyword search criteria
                if (!String.IsNullOrEmpty(sKeywordValue))
                {
                    AddKeywordFieldToQuery(sKeywordValue);
                }

                //Add each field from field list to query
                if (listFields != null)
                {
                    for (int i = 0; i < listFields.Count; i++)
                    {
                        AddFieldToQuery(listFields[i]);
                    }
                }

                //Save whether search criteria was blank. We will prompt a confirmation to execute a blank search.
                bSaveQueryHasData = bQueryHasData;

                //Add default query if no search criteria specified and we are continuing anyway
                if (!bQueryHasData)
                {
                    //No criteria specified, generate default query
                    AddDefaultQuery();
                }

                //Restrict search based on certain Search Types
                if (!String.IsNullOrEmpty(sSearchType))
                {
                    AddSearchTypeRestrictions(sSearchType);
                }

                //Restrict search with list of ObjectIDs
                if (!String.IsNullOrEmpty(sObjectId))
                {
                    AddObjectRestrictionsToQuery(sObjectId);
                }

                xWriter.WriteEndElement(); //end outtermost ExpressionSet

                //Write Closing of XML document
                xWriter.WriteEndDocument();

                xWriter.Close();
            }
            catch (Exception ex)
            {
                xWriter.Close();
                String sMsg = "QUERY: " + sbQuery.ToString() + "\n ERROR: " + ex.ToString();
                //trace.TraceError(sMsg);

                if (String.IsNullOrEmpty(m_sError))
                {
                    //Default Error
                   // m_sError = Resources.SearchResStrings.Error_Query_Build;
                }

                string sDisplayMsg = m_sError;

                if (!String.IsNullOrEmpty(m_sErrorDetail))
                {
                    sDisplayMsg += " ";
                    sDisplayMsg += m_sErrorDetail;
                }

                //SearchError.SetError(sDisplayMsg);
                //Clear out query on error
                sbQuery.Remove(0, sbQuery.Length);

                //Since we're hit an exception, set an error message, and cleared the query
                //We can set allowEmptyQuery to true to prevent the now unnecessary "No criteria was specified" pop-up
                bAllowEmptyQuery = true;
            }

            // Throw error if query was empty and we are not allowing an empty query
            if (!bAllowEmptyQuery && !bSaveQueryHasData)
            {
                throw new Exception(SearchExceptionStrings.NoCriteriaSpecified);
            }

            return sbQuery.ToString();
        }

        private void AddSearchTypeRestrictions(String sSearchType)
        {

            if (sSearchType == SearchTypeKeys.ACL)
            {
                //CQSDR00036748: "My Files/SharePoint" search type should just be "My Files" to avoid confusion
                //ACL searching can return SharePoint content as well as files, if the PlatformType of Files is not specified int the query
                ArrayList numList = new ArrayList(1);
                numList.Add(SearchPlatformTypeKeys.FILES);
                WriteExpression(QueryXmlDefines.OR, SearchPropKeys.PLATFORMTYPE, QueryXmlDefines.INT32, QueryXmlDefines.EQUALS, numList);
            }
        }

        private void AddObjectRestrictionsToQuery(String sObjectId)
        {
            //If searching Everything, do not add object restrictions
            if (sObjectId != SearchObjectKeys.EVERYTHING)
            {
                PresentationObject obj = sp.Model.GetObject(sObjectId);
                if (obj != null)
                {
                    ArrayList objList = new ArrayList();

                    //Recursively generate a list of ObjectIDs
                    if (obj.AssociatedGroup is PresentationGroup)
                    {
                        AddObjectRestrictionsToQueryHelper((PresentationGroup)obj.AssociatedGroup, ref objList);
                    }
                    else
                    {
                        objList.Add(obj.ID);
                    }

                    //Now OR these Object IDs into the query
                    WriteExpression(QueryXmlDefines.OR, SearchPropKeys.ITEMTYPE, QueryXmlDefines.INT32, QueryXmlDefines.EQUALS, objList);
                }
            }
        }

        private void AddObjectRestrictionsToQueryHelper(PresentationGroup grp, ref ArrayList objList)
        {
            foreach (PresentationGroup grpInner in grp.Groups)
            {
                AddObjectRestrictionsToQueryHelper(grpInner, ref objList);
            }
            foreach (PresentationGroupItem item in grp.GroupItems)
            {
                if (item.AssociatedObject != null)
                {
                    objList.Add(item.AssociatedObject.ID);
                }
            }
        }

        private void AddDefaultQuery()
        {
            //Default query
            DateTime dtDate = new DateTime(2038, 1, 1);
            DateTime dtNow = DateTime.Now;
            ArrayList dateList = new ArrayList();
            CultureInfo ci = new CultureInfo("en-US");

            if (dtNow > dtDate)
            {
                //need to add 1 year to be safe
                dtDate = dtNow.AddYears(1);
            }

            dateList.Add(dtDate.ToString("s", ci));

            WriteExpression(QueryXmlDefines.OR, SearchPropKeys.DATE, QueryXmlDefines.STRING, QueryXmlDefines.LESS_EQUAL, dateList);

        }

        private void AddKeywordFieldToQuery(String sKeywordValue)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //Fields that we will search against for Keyword Searches
            ArrayList listFields = new ArrayList(3);
            listFields.Add(SearchPropKeys.KEYWORD);

            ArrayList searchTermList = new ArrayList();
            ArrayList searchTermNotList = new ArrayList();
            ArrayList searchTermBeginsWithList = new ArrayList();
            ArrayList searchTermEndsWithList = new ArrayList();
            ParseStringField(SearchPropKeys.KEYWORD, sKeywordValue, ref searchTermList, ref searchTermNotList, ref searchTermBeginsWithList, ref searchTermEndsWithList);

            Dictionary<string, ArrayList> dictInput = new Dictionary<string, ArrayList>();
            dictInput.Add(QueryXmlDefines.CONTAINS, searchTermList);
            dictInput.Add(QueryXmlDefines.DOES_NOT_CONTAIN, searchTermNotList);
            dictInput.Add(QueryXmlDefines.BEGINS_WITH, searchTermBeginsWithList);
            dictInput.Add(QueryXmlDefines.ENDS_WITH, searchTermEndsWithList);

            WriteMultiTagExpression(listFields, QueryXmlDefines.OR, QueryXmlDefines.STRING, dictInput);

        }

        private void AddFieldToQuery(ISearchFieldControl sfc)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (sfc != null)
            {
                String sTag = sfc.FieldTag;

                switch (sfc.FieldType)
                {
                    case FieldItemType.Address:
                        List<KeyValuePair<string, ArrayList>> listAddrsAuto;
                        String sAddrsMan;
                        sfc.GetAddressValue(out listAddrsAuto, out sAddrsMan);
                        ProcessAddressField(sTag, ref listAddrsAuto, sAddrsMan);
                        break;

                    case FieldItemType.DateTime:
                        String sSpecifier;
                        DateTime dtDate;
                        sfc.GetDateTimeValue(out sSpecifier, out dtDate);
                        ProcessDateTimeField(sTag, sSpecifier, dtDate);
                        break;

                    case FieldItemType.NumberType:
                        String sRange;
                        String sNumber;
                        sfc.GetNumberValue(out sRange, out sNumber);
                        ProcessNumberField(sTag, sRange, sNumber);
                        break;

                    case FieldItemType.BooleanType:
                        String sBoolVal;
                        sfc.GetBooleanValue(out sBoolVal);
                        ProcessBooleanField(sTag, sBoolVal);
                        break;

                    case FieldItemType.ComboType:
                        String sComboVal;
                        sfc.GetComboValue(out sComboVal);
                        ProcessComboField(sTag, sComboVal);
                        break;

                    case FieldItemType.String:
                    default:
                        String sValue;
                        sfc.GetStringValue(out sValue);
                        ProcessStringField(sTag, sValue);
                        break;

                    case FieldItemType.Removed:
                        //Do not include removed fields in the query
                        break;
                }
            }
        }

        private void ProcessStringField(String sTag, String sValue)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (sTag == SearchPropKeys.FULLTEXT)
            {
                //Handle special FULLTEXT string case
                WriteFullTextExpression(sValue);
            }
            else
            {
                PresentationProperty prop = sp.GetPropFromTag(sTag);
                if (prop != null)
                {
                    ArrayList listFields = new ArrayList(1);
                    listFields.Add(sTag);

                    //Handle normal string fields
                    ArrayList searchTermList = new ArrayList();
                    ArrayList searchTermNotList = new ArrayList();
                    ArrayList searchTermBeginsWithList = new ArrayList();
                    ArrayList searchTermEndsWithList = new ArrayList();

                    ParseStringField(sTag, sValue, ref searchTermList, ref searchTermNotList, ref searchTermBeginsWithList, ref searchTermEndsWithList);

                    //Special case for Source Location property (NPMID=58), remove trailing slash before searching
                    if (sTag == SearchPropKeys.SOURCELOCATION)
                    {
                        RemoveTrailingSlash(ref searchTermList);
                        RemoveTrailingSlash(ref searchTermNotList);
                    }

                    Dictionary<string, ArrayList> dictInput = new Dictionary<string, ArrayList>();

                    //Some string types, such as ENTRYID and TRANSACTIONID, are EQUALS, all others are CONTAINS
                    // For GOS, if EQUALS is defined, we'll use it, otherwise use CONTAINS
                    PresentationSearchOperation op = prop.GetSearchOperation(SearchOperationType.EQUALS);
                    if (op != null)
                    {
                        dictInput.Add(QueryXmlDefines.EQUALS, searchTermList);
                        dictInput.Add(QueryXmlDefines.NOT_EQUAL, searchTermNotList);
                    }
                    else
                    {
                        dictInput.Add(QueryXmlDefines.CONTAINS, searchTermList);
                        dictInput.Add(QueryXmlDefines.DOES_NOT_CONTAIN, searchTermNotList);
                    }

                    //Add wildcards if supported
                    //Check if wildcards are supported for the prop before allowing
                    if (searchTermBeginsWithList.Count > 0)
                    {
                        if (prop.GetSearchOperation(SearchOperationType.BEGINS_WITH) != null)
                            dictInput.Add(QueryXmlDefines.BEGINS_WITH, searchTermBeginsWithList);
                        else
                            ThrowWildcardException(sTag);
                    }
                    else if (searchTermEndsWithList.Count > 0)
                    {
                        if (prop.GetSearchOperation(SearchOperationType.ENDS_WITH) != null)
                            dictInput.Add(QueryXmlDefines.ENDS_WITH, searchTermEndsWithList);
                        else
                            ThrowWildcardException(sTag);
                    }

                    WriteMultiTagExpression(listFields, QueryXmlDefines.OR, QueryXmlDefines.STRING, dictInput);

                }
                else
                {
                    //trace.TraceError("Encountered unknown property. Unable to add prop to query. Tag: " + sTag);
                }
            }

        }

        private void RemoveTrailingSlash(ref ArrayList searchTermList)
        {
            for (int i = 0; i < searchTermList.Count; i++)
            {
                //Trim trailing slashes 
                searchTermList[i] = ((string)searchTermList[i]).TrimEnd(new char[] { '\\', '/' });
            }
        }


        private void ThrowWildcardException(string sMoreInfo)
        {
            //raise error if wildcards were used inappropriately
            //m_sErrorDetail = Resources.SearchResStrings.Error_Query_Wildcard_Invalid;
            Exception ex = new Exception("Resources.SearchResStrings.Error_Query_Wildcard_Invalid" + ": " + sMoreInfo);
            throw (ex);
        }

        private void ThrowValidationException(string sUserMsg, string sMoreInfo)
        {
            //raise validation error
            m_sErrorDetail = sUserMsg;
            Exception ex = new Exception(sUserMsg + ": " + sMoreInfo);
            throw (ex);
        }

        private void ParseStringField(String sTag, String sValue, ref ArrayList searchTermList, ref ArrayList searchTermNotList, ref ArrayList searchTermBeginsWithList, ref ArrayList searchTermEndsWithList)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //Track items that are valid search terms that will need wildcard processing later
            ArrayList listRemaining = new ArrayList();

            //Remove leading and trailing whitespace
            sValue = sValue.Trim();

            //Move Phrases from source string to searchTermList array
            int startIndex = 0;
            int endIndex = 0;
            while (((startIndex = sValue.IndexOf('"')) != -1) && endIndex != -1)
            {
                endIndex = sValue.IndexOf('"', startIndex + 1);
                if (endIndex != -1)
                {
                    //We found a phrase

                    //Check if the phrase should be NOT'd
                    bool bNot = false;
                    if ((startIndex - 1 >= 0) /*prevent access violation*/
                        && (sValue[startIndex - 1] == '-'))
                    {
                        bNot = true;
                    }

                    if (bNot)
                    {
                        //Add the phrase to the searchTermNotList array,
                        //Then remove it (and surrounding quotes) from the source string
                        String sTerm = sValue.Substring(startIndex + 1, (endIndex - (startIndex + 1)));
                        if (sTerm != String.Empty)
                        {
                            searchTermNotList.Add(sTerm);
                        }
                        sValue = sValue.Remove(startIndex - 1, ((endIndex + 1) - (startIndex - 1)));
                    }
                    else
                    {
                        //Add the phrase to the listRemaining array,
                        //Then remove it (and surrounding quotes) from the source string
                        String sTerm = sValue.Substring(startIndex + 1, (endIndex - (startIndex + 1)));
                        if (sTerm != String.Empty)
                        {
                            listRemaining.Add(sTerm);
                        }
                        sValue = sValue.Remove(startIndex, ((endIndex + 1) - startIndex));
                    }

                }
            }

            //Now split on the space character to get remaining search terms for processing
            // CQSDR00041558, EMAIL-2958 : Need to also support the "ideographic space" character (unicode hex 3000), used commonly in Japanese, as a search term delimiter.
            char[] charArray = { ' ', '\u3000' };
            ArrayList listTemp = new ArrayList(sValue.Split(charArray, StringSplitOptions.RemoveEmptyEntries));

            if (sTag == SearchPropKeys.SENDER_DOMAIN || sTag == SearchPropKeys.RECIPIENT_DOMAIN)
            {
                //Sender and Recipient Domain fields only support CONTAINS searching.
                //Do not part out '-' NOT operator or '*' wildcard characters.
                //Wildcard characters for the domain search are passed through as-is
                searchTermList.AddRange(listTemp);
            }
            else
            {
                //NOT processing for single word terms
                foreach (string term in listTemp)
                {
                    if (term[0] == '-')
                    {
                        //Found a NOT term
                        if (term.Length > 1)
                        {
                            searchTermNotList.Add(term.Substring(1));
                        }
                    }
                    else
                    {
                        listRemaining.Add(term);
                    }
                }

                //Wild card processing
                ArrayList listBeginsWith = new ArrayList();
                ArrayList listEndsWith = new ArrayList();
                ParseOutWildCardTerms(listRemaining, ref searchTermBeginsWithList, ref searchTermEndsWithList, ref searchTermList);
            }
        }


        private void ParseOutWildCardTerms(ArrayList listInput, ref ArrayList listBeginsWith, ref ArrayList listEndsWith, ref ArrayList listOther)
        {
            foreach (string term in listInput)
            {
                //Check for wildcard
                if (term.Contains("*"))
                {
                    if (term.IndexOf('*') == 0)
                    {
                        //Ends with wildcard
                        //Remove wildcard char and check for more wildcards
                        string sNewTerm = term;
                        if (sNewTerm.Length > 1)
                        {
                            sNewTerm = sNewTerm.Remove(0, 1);
                        }
                        if (sNewTerm.Contains("*"))
                        {
                            //Error - only one wildcard character supported per term
                            ThrowWildcardException(term);
                        }
                        else
                        {
                            //Good - Add term
                            listEndsWith.Add(sNewTerm);
                        }
                    }
                    else if (term.LastIndexOf('*') == (term.Length - 1))
                    {
                        //Begins with wildcard
                        //Remove wildcard char and check for more wildcards
                        string sNewTerm = term.Remove(term.Length - 1, 1);
                        if (sNewTerm.Contains("*"))
                        {
                            //Error - only one wildcard character supported per term
                            ThrowWildcardException(term);
                        }
                        else
                        {
                            //Good - Add term
                            listBeginsWith.Add(sNewTerm);
                        }
                    }
                    else
                    {
                        //Error - Wildcard was in the middle - not supported
                        ThrowWildcardException(term);
                    }
                }
                else
                {
                    listOther.Add(term);
                }
            }

        }
        private void ProcessAddressField(String sTag, ref List<KeyValuePair<string, ArrayList>> listAddrsAuto, String sAddrsMan)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            PresentationProperty prop = sp.GetPropFromTag(sTag);
            if (prop != null)
            {
                ArrayList addressListMan = new ArrayList();
                ArrayList addressListAuto = new ArrayList();

                ///////////////////////////////////////////////
                //Process Manually entered addresses first
                ///////////////////////////////////////////////

                //LIMITATION: Manually added Addresses are delimited by a ";"(Exchange) or a ","(Domino)
                //We are unable to manually search addresses with these characters. Users must use Address finder page.
                if (m_EmailEnv == SearchMailEnv.EXCHANGE)
                    addressListMan.AddRange(sAddrsMan.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                else if (m_EmailEnv == SearchMailEnv.NOTES)
                    addressListMan.AddRange(sAddrsMan.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                else
                    addressListMan.AddRange(sAddrsMan.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));


                for (int i = 0; i < addressListMan.Count; i++)
                {
                    addressListMan[i] = ((String)addressListMan[i]).Trim();
                }

                ///////////////////////////////////////////////
                //Process Auto-generated addresses last
                ///////////////////////////////////////////////

                if (listAddrsAuto != null)
                {
                    foreach (KeyValuePair<string, ArrayList> kvp in listAddrsAuto)
                    {
                        if (kvp.Value != null)
                        {
                            foreach (string sAddr in (ArrayList)kvp.Value)
                            {
                                //EMAIL-732 - removing tolower from client side as this causes problems if default language is not english
                                addressListAuto.Add(sAddr);
                            }
                        }
                    }
                }

                ArrayList listFields = new ArrayList();
                //Handle special SENDERORRECIPIENT case
                if (sTag == SearchPropKeys.SENDERORRECIPIENT)
                {
                    //Search on both SENDER OR RECIPIENT
                    listFields.Add(SearchPropKeys.SENDER);
                    listFields.Add(SearchPropKeys.RECIPIENT);
                }
                else
                {
                    //Search on the Tag as is.
                    listFields.Add(sTag);
                }

                ArrayList searchTermBeginsWithList = new ArrayList();
                ArrayList searchTermEndsWithList = new ArrayList();
                ArrayList addressListMan_Remaining = new ArrayList();

                ParseOutWildCardTerms(addressListMan, ref searchTermBeginsWithList, ref searchTermEndsWithList, ref addressListMan_Remaining);

                //Add Picked addresses as QueryXmlDefines.EQUALS
                Dictionary<string, ArrayList> dictInput = new Dictionary<string, ArrayList>();
                dictInput.Add(QueryXmlDefines.EQUALS, addressListAuto);

                //Add Manually entered address as QueryXmlDefines.CONTAINS, unless only QueryXmlDefines.EQUALS is supported 
                //TODO: ensure PXML contains only EQUALS search operation for the Owner prop
                if (prop.GetSearchOperation(SearchOperationType.CONTAINS) != null)
                {
                    //For most Address-type fields, support wildcards:
                    dictInput.Add(QueryXmlDefines.CONTAINS, addressListMan_Remaining);
                }
                else
                {
                    if (prop.GetSearchOperation(SearchOperationType.EQUALS) != null)
                    {
                        dictInput[QueryXmlDefines.EQUALS].AddRange(addressListMan_Remaining);
                    }
                }

                //Add wildcards if supported
                //Check if wildcards are supported for the prop before allowing
                if (searchTermBeginsWithList.Count > 0)
                {
                    if (prop.GetSearchOperation(SearchOperationType.BEGINS_WITH) != null)
                        dictInput.Add(QueryXmlDefines.BEGINS_WITH, searchTermBeginsWithList);
                    else
                        ThrowWildcardException(sTag);
                }
                else if (searchTermEndsWithList.Count > 0)
                {
                    if (prop.GetSearchOperation(SearchOperationType.ENDS_WITH) != null)
                        dictInput.Add(QueryXmlDefines.ENDS_WITH, searchTermEndsWithList);
                    else
                        ThrowWildcardException(sTag);
                }


                WriteMultiTagExpression(listFields, QueryXmlDefines.OR, QueryXmlDefines.STRING, dictInput);
            }
            else
            {
                //trace.TraceError("Encountered unknown property. Unable to add prop to query. Tag: " + sTag);
            }
        }



        private void ProcessDateTimeField(String sTag,
                                    String sSpecifier,
                                    DateTime dtDate)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (dtDate != DateTime.MinValue)
            {
                //Validate that a specifier has been set
                if (String.IsNullOrEmpty(sSpecifier))
                {
                    //ThrowValidationException(Resources.SearchResStrings.Error_Query_Validate_Date_Qualifier, sTag);
                }
                else
                {
                    DateTime dtDate2;
                    ArrayList dateList = new ArrayList();
                    ArrayList dateList2 = new ArrayList();
                    CultureInfo ci = new CultureInfo("en-US");

                    switch (sSpecifier)
                    {
                        case "ON":
                            //Date range is one day
                            dtDate2 = dtDate.AddDays(1);
                            //Convert to GMT (Universal) time
                            dtDate = dtDate.ToUniversalTime();
                            dtDate2 = dtDate2.ToUniversalTime();
                            //Use DateTime format: (u) Universal sortable (invariant): 2006-04-17 21:29:09Z
                            dateList.Add(dtDate.ToString("s", ci));
                            dateList2.Add(dtDate2.ToString("s", ci));

                            //Since the outer level is AND, this creates unnecessary nesting
                            //WriteExpression(QueryXmlDefines.AND, sTag, QueryXmlDefines.STRING, QueryXmlDefines.GREATER_EQUAL, dateList, QueryXmlDefines.LESS_THAN, dateList2);

                            WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.STRING, QueryXmlDefines.GREATER_EQUAL, dateList);
                            WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.STRING, QueryXmlDefines.LESS_THAN, dateList2);
                            break;

                        case "ONORAFTER":
                            //Convert to GMT (Universal) time
                            dtDate = dtDate.ToUniversalTime();
                            //Use DateTime format: (u) Universal sortable (invariant): 2006-04-17 21:29:09Z
                            dateList.Add(dtDate.ToString("s", ci));

                            WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.STRING, QueryXmlDefines.GREATER_EQUAL, dateList);
                            break;

                        case "ONORBEFORE":
                            //need to add 1 day so this day is inclusive
                            dtDate = dtDate.AddDays(1);
                            //Convert to GMT (Universal) time
                            dtDate = dtDate.ToUniversalTime();
                            //Use DateTime format: (u) Universal sortable (invariant): 2006-04-17 21:29:09Z
                            dateList.Add(dtDate.ToString("s", ci));

                            WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.STRING, QueryXmlDefines.LESS_THAN, dateList);
                            break;

                        default:
                            break;
                    }

                }



            }

        }

        private void ProcessNumberField(String sTag,
                            String sSpecifier,
                            String sNumber)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            ArrayList numList = new ArrayList(1);

            if (!String.IsNullOrEmpty(sSpecifier) &&
                !String.IsNullOrEmpty(sNumber))
            {
                PresentationProperty prop = sp.GetPropFromTag(sTag);
                if (prop != null)
                {
                    string dataType = QueryXmlDefines.INT32;
                    if (prop.DataType == FieldDataType.Int64)
                    {
                        dataType = QueryXmlDefines.INT64;
                    }

                    switch (sSpecifier)
                    {
                        case "EQUALS":
                            numList.Add(sNumber);
                            WriteExpression(QueryXmlDefines.OR, sTag, dataType, QueryXmlDefines.EQUALS, numList);
                            break;

                        case "GREATERTHAN":
                            numList.Add(sNumber);
                            WriteExpression(QueryXmlDefines.OR, sTag, dataType, QueryXmlDefines.GREATER_THAN, numList);
                            break;

                        case "LESSTHAN":
                            numList.Add(sNumber);
                            WriteExpression(QueryXmlDefines.OR, sTag, dataType, QueryXmlDefines.LESS_THAN, numList);
                            break;

                        default:
                            break;
                    }
                }

            }

        }

        private void ProcessBooleanField(String sTag, String sValue)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (!String.IsNullOrEmpty(sValue))
            {
                PresentationProperty prop = sp.GetPropFromTag(sTag);
                if (prop != null)
                {
                    if (prop.DataType == FieldDataType.Boolean)
                    {
                        //Handle boolean dataTypes
                        ArrayList searchTermList = new ArrayList();
                        searchTermList.Add(sValue);
                        WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.BOOLEAN, QueryXmlDefines.EQUALS, searchTermList);
                    }
                    else if (prop.DataType == FieldDataType.Int32 ||
                             prop.DataType == FieldDataType.Int64)
                    {
                        //Handle boolean fieldDisplayTypes that have a integer dataType.
                        //Should be "true" if the value is greater than 0

                        string dataType = QueryXmlDefines.INT32;
                        if (prop.DataType == FieldDataType.Int64)
                        {
                            dataType = QueryXmlDefines.INT64;
                        }

                        ArrayList numList = new ArrayList(1);
                        numList.Add("0");

                        String sSearchOp = String.Empty;

                        if (0 == String.Compare(sValue, "TRUE", true))
                        {
                            sSearchOp = QueryXmlDefines.GREATER_THAN;
                        }
                        else
                        {
                            sSearchOp = QueryXmlDefines.EQUALS;
                        }

                        if (sTag == SearchPropKeys.ANYUNINDEXEDCONTENT)
                        {
                            //Special case to find all Unindexable Content. (Encrypted or PwdProtectedAttach or IndexingError)
                            //USE MULTITAGEXPRESSION                            
                            ArrayList listFields = new ArrayList(3);
                            listFields.Add(SearchPropKeys.ENCRYPTED);
                            listFields.Add(SearchPropKeys.PWDPROTECTEDATTACH);
                            listFields.Add(SearchPropKeys.INDEXINGERROR);

                            Dictionary<string, ArrayList> dictInput = new Dictionary<string, ArrayList>();
                            dictInput.Add(sSearchOp, numList);

                            if (0 == String.Compare(sValue, "TRUE", true))
                            {
                                //Need to OR the three properties together for the positive case
                                WriteMultiTagExpression(listFields, QueryXmlDefines.OR, QueryXmlDefines.STRING, dictInput);
                            }
                            else
                            {
                                //Need to AND the three properties together for the negative case
                                WriteMultiTagExpression(listFields, QueryXmlDefines.AND, QueryXmlDefines.STRING, dictInput);
                            }
                        }
                        else
                        {
                            WriteExpression(QueryXmlDefines.OR, sTag, dataType, sSearchOp, numList);
                        }

                    }
                }
            }
        }


        private void ProcessComboField(String sTag, String sValue)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (!String.IsNullOrEmpty(sValue))
            {
                if (sTag == SearchPropKeys.ITEMTYPE)
                {
                    //Special case ItemType because it could be a grouping of items
                    AddObjectRestrictionsToQuery(sValue);
                }
                else
                {
                    ArrayList searchTermList = new ArrayList();

                    searchTermList.Add(sValue);

                    WriteExpression(QueryXmlDefines.OR, sTag, QueryXmlDefines.INT32, QueryXmlDefines.EQUALS, searchTermList);
                }


            }
        }


        private void WriteMultiTagExpression(ArrayList listTags,
                                    String sLogicalOp,
                                    String sDataType,
                                    Dictionary<string, ArrayList> dictInput)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            //Determine if we need to write a wrapper ExpressionsSet
            bool bWriteExpressionSet = false;

            int nValCount = 0;
            foreach (KeyValuePair<string, ArrayList> entry in dictInput)
            {
                nValCount += ((ArrayList)entry.Value).Count;
            }

            //Need ExpressionSet if we have more than one value, or more than one tag and at least one value
            if ((nValCount > 1) ||
                (listTags.Count > 1) && (nValCount > 0))
            {
                bWriteExpressionSet = true;
                xWriter.WriteStartElement(QueryXmlDefines.EXPRESSIONSET);
                xWriter.WriteAttributeString(QueryXmlDefines.LOGICALOPERATOR, sLogicalOp);
            }

            foreach (String sTag in listTags)
            {
                foreach (KeyValuePair<string, ArrayList> entry in dictInput)
                {
                    string sSearchOp = (string)entry.Key;
                    ArrayList listTerms = (ArrayList)entry.Value;
                    foreach (string term in listTerms)
                    {
                        WriteSimpleAttributeExpression(sTag, sSearchOp, sDataType, term);
                    }
                }
            }

            if (bWriteExpressionSet)
            {
                xWriter.WriteEndElement(); //end ExpressionSet
            }
        }

        private void WriteExpression(String sLogicalOp,
                                    String sAttribute,
                                    String sDataType,
                                    String sSearchOp,
                                    ArrayList listTerms)
        {
            WriteExpression(sLogicalOp,
                            sAttribute,
                            sDataType,
                            sSearchOp,
                            listTerms,
                            "",
                            null);

        }


        private void WriteExpression(String sLogicalOp,
                                    String sAttribute,
                                    String sDataType,
                                    String sSearchOp,
                                    ArrayList listTerms,
                                    String sSearchOpNot,
                                    ArrayList listTermsNot)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            int nCountTerms = 0;
            if (listTerms != null)
            {
                nCountTerms = listTerms.Count;
            }
            if (listTermsNot != null)
            {
                nCountTerms += listTermsNot.Count;
            }

            if (nCountTerms > 1)
            {
                //If multiple search terms, need to embed in a nested ExpressionSet
                xWriter.WriteStartElement(QueryXmlDefines.EXPRESSIONSET);
                xWriter.WriteAttributeString(QueryXmlDefines.LOGICALOPERATOR, sLogicalOp);
            }

            if (listTerms != null)
            {
                foreach (String searchTerm in listTerms)
                {
                    WriteSimpleAttributeExpression(sAttribute, sSearchOp, sDataType, searchTerm);
                }
            }

            if (listTermsNot != null)
            {
                foreach (String searchTerm in listTermsNot)
                {
                    WriteSimpleAttributeExpression(sAttribute, sSearchOpNot, sDataType, searchTerm);
                }
            }

            if (nCountTerms > 1)
            {
                xWriter.WriteEndElement(); //end ExpressionSet
            }

        }

        private void WriteSimpleAttributeExpression(string sAttribute, string sSearchOp, string sDataType, string searchTerm)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            PresentationProperty prop = sp.GetPropFromTag(sAttribute);
            if (prop != null)
            {
                xWriter.WriteStartElement(QueryXmlDefines.SIMPLEATTRIBUTEEXPRESSION);

                if (prop.IsPresentationOnlyProperty)
                {
                    xWriter.WriteAttributeString(QueryXmlDefines.NPMPROPERTYID, prop.PresentationPropId.ToString());
                }
                else
                {
                    //TODO: determine if we should pass in objectID
                    //xWriter.WriteAttributeString(QueryXmlDefines.NPMOBJECTID, prop.NPMObjectID.ToString());
                    xWriter.WriteAttributeString(QueryXmlDefines.NPMPROPERTYID, prop.NPMPropID.ToString());
                }

                xWriter.WriteAttributeString(QueryXmlDefines.DISPLAYNAME, prop.DisplayName);
                xWriter.WriteAttributeString(QueryXmlDefines.SEARCHOPERATION, sSearchOp);
                xWriter.WriteAttributeString(QueryXmlDefines.DATATYPE, sDataType);
                xWriter.WriteAttributeString(QueryXmlDefines.CASESENSITIVE, "false"); //always case INsensitve
                xWriter.WriteValue(searchTerm);
                xWriter.WriteEndElement(); // end SimpleAttributeExpression

                //Track that query is not blank
                bQueryHasData = true;
            }
            else
            {
                //trace.TraceError("Encountered unknown property. Unable to add prop to query. Attribute tag: " + sAttribute);
            }
        }

        //WriteFullTextExpression(sValue)
        private void WriteFullTextExpression(String sValue)
        {
            //ExSearchTrace trace = new ExSearchTrace();

            if (!String.IsNullOrEmpty(sValue))
            {
                xWriter.WriteStartElement(QueryXmlDefines.FULLTEXTEXPRESSION);
                xWriter.WriteValue(sValue);
                xWriter.WriteEndElement(); // end FullTextExpression

                //Track that query is not blank
                bQueryHasData = true;
            }
        }
    }

    public static class QueryXmlDefines
    {
        public const string STRING = "string";
        public const string INT32 = "int32";
        public const string INT64 = "int64";
        public const string BOOLEAN = "boolean";
        public const string DATE = "date";

        public const string BEGINS_WITH = "BEGINS_WITH";
        public const string ENDS_WITH = "ENDS_WITH";
        public const string CONTAINS = "CONTAINS";
        public const string DOES_NOT_CONTAIN = "DOES_NOT_CONTAIN";
        public const string EQUALS = "EQUALS";
        public const string NOT_EQUAL = "NOT_EQUAL";
        public const string GREATER_THAN = "GREATER_THAN";
        public const string GREATER_EQUAL = "GREATER_EQUAL";
        public const string LESS_THAN = "LESS_THAN";
        public const string LESS_EQUAL = "LESS_EQUAL";

        public const string AND = "AND";
        public const string OR = "OR";

        public const string NPMOBJECTID = "npmObjectId";
        public const string NPMPROPERTYID = "npmPropertyId";
        public const string DISPLAYNAME = "displayName";
        public const string SEARCHOPERATION = "searchOperation";
        public const string DATATYPE = "dataType";
        public const string CASESENSITIVE = "caseSensitive";
        //public const string METADATATAGNAME = "metadataTagName";

        public const string FULLTEXTEXPRESSION = "FullTextExpression";
        public const string EXPRESSIONSET = "ExpressionSet";
        public const string SIMPLEATTRIBUTEEXPRESSION = "SimpleAttributeExpression";
        public const string LOGICALOPERATOR = "logicalOperator";
    };
    
}
