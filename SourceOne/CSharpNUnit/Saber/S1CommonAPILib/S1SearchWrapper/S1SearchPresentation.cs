///////////////////////////////////////////////////////////////////////////////////////
//		Copyright © 1998 - 2007 EMC Corporation. All rights reserved.
//		This software contains the intellectual property of EMC Corporation
//		or is licensed to EMC Corporation from third parties. Use of this software
//		and the intellectual property contained therein is expressly limited to
//		the terms and conditions of the License Agreement under which it is
//		provided by or on behalf of EMC.
//						  EMC Corporation,
//					      176 South St.,
//					  Hopkinton, MA  01748.
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
//using GOSPresentationCommon.Model;

namespace Saber.S1CommonAPILib.S1SearchWrapper
{
    /// <summary>
    /// Keys for storing Session objects
    /// </summary>
    public class SearchPresentation
    {
        //ExSearchTrace m_trace = new ExSearchTrace();

        //Lazy Loaded
        GOSPresentationModel m_presentationModel = null;
        System.Resources.ResourceManager m_rm = null;
        System.Globalization.CultureInfo m_ci = null;

        public SearchPresentation()
        {
            //Constructor logic
        }

        public GOSPresentationModel Model
        {
            get
            {
                if (m_presentationModel == null)
                {
                    //if (HttpContext.Current.Session[SearchSessionKeys.PresentationModel] == null)
                    //{
                        m_presentationModel = new GOSPresentationModel();

                        ////Attach delegate logging methods
                        //m_presentationModel.OnLogException += new LogException(model_OnLogException);
                        //m_presentationModel.OnLogInformation += new LogInformation(model_OnLogInformation);
                        //m_presentationModel.OnLogVerbose += new LogVerbose(model_OnLogVerbose);

                        String NPMXml = String.Empty;
                        String PresentationXml = String.Empty;
                        //String ExtensibilityXml = String.Empty;

                        //SearchWrapper oSearch = new SearchWrapper();

                        ////NPM initialization
                        //if (HttpContext.Current.Application[SearchApplicationKeys.NPMXml] != null)
                        //{
                        //    //Use existing NPM
                        //    NPMXml = (string)HttpContext.Current.Application[SearchApplicationKeys.NPMXml];
                        //}
                        //else
                        //{
                        //    //Load New NPM

                        //    //Check for local NPM override
                        //    if (ConfigurationManager.AppSettings[SearchApplicationKeys.LoadLocalNPMFileName] != null)
                        //    {
                        //        string sFileName = ConfigurationManager.AppSettings[SearchApplicationKeys.LoadLocalNPMFileName];
                        //        if (!String.IsNullOrEmpty(sFileName))
                        //        {
                        //            //Load local NPM override from App_Data folder
                        //            NPMXml = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("App_Data/" + sFileName));
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //Load NPM from web service   
                        //        NPMXml = oSearch.GetXmlFile(ExSearch.ExXmlFile.NormalizedPropertyMapXml);
                        //        HttpContext.Current.Application[SearchApplicationKeys.NPMXml] = NPMXml;
                        //    }
                        //}

                        ////PXML initialization
                        //if (HttpContext.Current.Application[SearchApplicationKeys.PresentationXml] != null)
                        //{
                        //    //Use existing PXML
                        //    PresentationXml = (string)HttpContext.Current.Application[SearchApplicationKeys.PresentationXml];
                        //}
                        //else
                        //{
                        //    //Load New PXML

                        //    //Check for local PXML override
                        //    if (ConfigurationManager.AppSettings[SearchApplicationKeys.LoadLocalPXMLFileName] != null)
                        //    {
                        //        string sFileName = ConfigurationManager.AppSettings[SearchApplicationKeys.LoadLocalPXMLFileName];
                        //        if (!String.IsNullOrEmpty(sFileName))
                        //        {
                        //            //Load local PXML override from App_Data folder
                        //            PresentationXml = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("App_Data/" + sFileName));
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //Load PXML from web service   
                        //        PresentationXml = oSearch.GetXmlFile(ExSearch.ExXmlFile.PresentationXml);
                        //        HttpContext.Current.Application[SearchApplicationKeys.PresentationXml] = PresentationXml;
                        //    }
                        //}
                        //Neil
                        Saber.S1CommonAPILib.S1SearchHelper.SearchWebService oSearch = new S1SearchHelper.SearchWebService();
                        PresentationXml = oSearch.GetXmlFile(localhost.ExXmlFile.PresentationXml).OuterXml;
                        NPMXml = oSearch.GetXmlFile(localhost.ExXmlFile.NormalizedPropertyMapXml).OuterXml;

                        //Load NPM and PXML
                        m_presentationModel.LoadFromPxml(NPMXml, PresentationXml);


                        //Extensibility XML initialization (if configured to load local extensibility file)                        
                        //if (HttpContext.Current.Application[SearchApplicationKeys.LoadExtensibilityXml] != null &&
                        //    (bool)HttpContext.Current.Application[SearchApplicationKeys.LoadExtensibilityXml])
                        //{
                        //    if (HttpContext.Current.Application[SearchApplicationKeys.ExtensibilityXml] != null)
                        //    {
                        //        ExtensibilityXml = (string)HttpContext.Current.Application[SearchApplicationKeys.ExtensibilityXml];
                        //    }
                        //    else
                        //    {
                        //        ExtensibilityXml = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("App_Data/Presentation.Extensions.Search.xml"));
                        //        HttpContext.Current.Application[SearchApplicationKeys.ExtensibilityXml] = ExtensibilityXml;
                        //    }

                        //    //if (!String.IsNullOrEmpty(ExtensibilityXml))
                        //    //{
                        //    //    //Load Extensibility
                        //    //    m_presentationModel.LoadExtensibility(ExtensibilityXml);
                        //    //}
                        //}

                        //Store in session for future references
                        //HttpContext.Current.Session[SearchSessionKeys.PresentationModel] = m_presentationModel;
                    //}
                    //else
                    //{
                    //    //Load from Application Object
                    //    m_presentationModel = (GOSPresentationModel)HttpContext.Current.Session[SearchSessionKeys.PresentationModel];
                    //}
                }

                return m_presentationModel;
            }
        }

        //private void model_OnLogVerbose(string sMessage)
        //{
        //    //Log the verbose type message using the respective client logging framework
        //    m_trace.TraceInfo("GOS VERBOSE: " + sMessage);
        //}

        //private void model_OnLogInformation(string sMessage)
        //{
        //    //Log the information type message using the respective client logging framework
        //    m_trace.TraceInfo("GOS INFO: " + sMessage);
        //}

        //private void model_OnLogException(Exception ex)
        //{
        //    //Log the exception using the respective client logging framework
        //    m_trace.TraceError("GOS ERROR: " + ex.ToString());
        //}

        //public void SavePresentationXml(string path)
        //{
        //    if (String.IsNullOrEmpty(path))
        //    {
        //        path = HttpContext.Current.Server.MapPath("App_Data/PXML.saved.xml");
        //    }

        //    Model.Save(path);

        //}

        //public string GetResourceString(string objId, PresentationViewType viewType, int npmPropId, string sPresOnlyPropId, string sDefault)
        //{
        //    string sRet = sDefault;

        //    try
        //    {
        //        PresentationObject obj = Model.GetObject(objId);
        //        if (obj != null)
        //        {
        //            string sResourceId = String.Empty;

        //            if (String.IsNullOrEmpty(sPresOnlyPropId))
        //            {
        //                switch (viewType)
        //                {
        //                    case PresentationViewType.Search:
        //                        PresentationCriteria crit = obj.GetCriterionView(0).GetPresentationCriteriaInstance(npmPropId);
        //                        if (crit != null)
        //                        {
        //                            sResourceId = crit.ResourceId;
        //                        }
        //                        break;
        //                    case PresentationViewType.Result:
        //                        PresentationColumn pCol = obj.GetResultView(0).GetPresentationColumnInstance(npmPropId);
        //                        if (pCol != null)
        //                        {
        //                            sResourceId = pCol.ResourceId;
        //                        }
        //                        break;
        //                    case PresentationViewType.Preview:
        //                        PresentationPreviewItem pItem = obj.GetPreviewView(0).GetPreviewItem(npmPropId);
        //                        if (pItem != null)
        //                        {
        //                            sResourceId = pItem.ResourceId;
        //                        }
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }


        //            if (String.IsNullOrEmpty(sResourceId))
        //            {
        //                //Could not located the Prop in the requested view for the requested object
        //                //Instead, just look up the resource id of the base property
        //                PresentationProperty prop = null;

        //                if (!String.IsNullOrEmpty(sPresOnlyPropId))
        //                {
        //                    //Look up Presentation Only property
        //                    prop = Model.GetPresentationOnlyProperty(sPresOnlyPropId);
        //                }
        //                else
        //                {
        //                    //Use the NPM-based property
        //                    prop = Model.GetProperty(npmPropId);

        //                }

        //                if (prop != null)
        //                {
        //                    sResourceId = prop.ResourceID;
        //                }
        //            }

        //            if (!String.IsNullOrEmpty(sResourceId))
        //            {
        //                sRet = GetResourceString(sResourceId, sDefault);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        m_trace.TraceError("Error loading object-specific prop resource for Resources.GOSPresentationStrings: " + ex.ToString());

        //        //Use the default string in case of error
        //        sRet = sDefault;
        //    }

        //    return sRet;
        //}

        //public string GetResourceString(string resId, string sDefault)
        //{
        //    string sRet = sDefault;

        //    try
        //    {
        //        if (m_rm == null)
        //        {
        //            m_rm = new System.Resources.ResourceManager("Resources.GOSPresentationStrings", System.Reflection.Assembly.Load("App_GlobalResources"));
        //        }

        //        if (m_ci == null)
        //        {
        //            m_ci = System.Threading.Thread.CurrentThread.CurrentCulture;
        //        }

        //        //Get resource by ID
        //        sRet = m_rm.GetString(resId, m_ci);

        //        if (String.IsNullOrEmpty(sRet))
        //        {
        //            //Could not load string from resource, use default
        //            sRet = sDefault;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        m_trace.TraceError("Error loading resource for Resources.GOSPresentationStrings: " + ex.ToString());

        //        //Use the default string in case of error
        //        sRet = sDefault;
        //    }

        //    return sRet;
        //}


        public PresentationProperty GetPropFromTag(string sTag)
        {
            // Use the sTags value of "<objectid>__<propid>__<displayname>"
            // Parse out the ObjectID and PropID
            // Get a reference to this prop in the presentationModel object

            PresentationProperty prop = null;

            try
            {
                int objectID = -1;
                int propID = -1;
                string name = null;

                string[] splitChars = { "__" };
                string[] arrParts = sTag.Split(splitChars, 4, StringSplitOptions.None);
                if (arrParts.Length == 4)
                {
                    //arrParts[0] should be the text "ID", needed because column names must begin with a letter, not a number

                    //Parse out the object and prop id's
                    if (!String.IsNullOrEmpty(arrParts[1]))
                    {
                        if (!Int32.TryParse(arrParts[1], out objectID))
                        {
                            objectID = 0;
                        }
                    }
                    if (!String.IsNullOrEmpty(arrParts[2]))
                    {
                        if (!Int32.TryParse(arrParts[2], out propID))
                        {
                            propID = -1;
                        }
                    }
                    if (!String.IsNullOrEmpty(arrParts[3]))
                    {
                        name = arrParts[3];
                    }

                    //Get the object based on the parsed out object and prop id's
                    if (objectID == 0)
                    {
                        prop = Model.GetProperty(propID);
                    }
                    else
                    {
                        prop = Model.GetProperty(propID, objectID);
                    }
                }
                else
                {
                    //Format does not match a mapped property. Must be a presentation-only prop.
                    prop = Model.GetPresentationOnlyProperty(sTag);
                }

            }
            catch (Exception ex)
            {
                //ExSearchTrace trace = new ExSearchTrace();
                //trace.TraceError("Exception caught in sfc.GetPropFromTag method: " + ex.ToString());
            }

            return prop;
        }

        //Returns a string tag to represent a property: <NPMObjectID__NPMPropID__DisplayName>
        public string GetTagFromProp(PresentationProperty prop)
        {
            string sTag = String.Empty;
            if (prop != null)
            {
                if (prop.IsPresentationOnlyProperty)
                {
                    sTag = prop.PresentationPropId;
                }
                else
                {
                    if (prop.NPMObjectID == -1)
                    {
                        sTag = "ID__0__" + prop.NPMPropID + "__" + prop.DisplayName.Replace(" ", "");
                    }
                    else
                    {
                        sTag = "ID__" + prop.NPMObjectID + "__" + prop.NPMPropID + "__" + prop.DisplayName.Replace(" ", "");
                    }
                }

            }
            return sTag;
        }




        #region LoadSearchCriteria

        //public void LoadSearchCriteria(string ObjectId, Telerik.Web.UI.RadTreeNodeCollection items)
        //{
        //    //items.Clear();

        //    //Recursive load
        //    PresentationObject obj = Model.GetObject(ObjectId);
        //    if (obj != null)
        //    {
        //        if (obj.AssociatedGroup is PresentationGroup)
        //        {
        //            PresentationGroup grp = obj.AssociatedGroup as PresentationGroup;
        //            LoadSearchCriteriaGroup(grp, items, 0);
        //        }

        //        //Root menu items    
        //        AddSearchCriteriaMenuItemsHelper(obj, items);
        //    }
        //}

        public bool IsObjAllowedForRole(PresentationObject obj, RoleType roleType)
        {
            //Allowed by default
            bool bAllowed = true;

            if (obj != null &&
                obj.Roles != null &&
                obj.Roles.Count > 0)
            {
                //Check if object is allowed for this search role
                //If the SearchRoles list is blank, then the obj is available for all roles
                if (obj.GetSearchRole(roleType) == null)
                {
                    bAllowed = false;
                }
            }

            return bAllowed;
        }

        public bool IsPropAllowedForRole(PresentationProperty prop, RoleType roleType)
        {
            //Allowed by default
            bool bAllowed = true;

            if (prop != null &&
                prop.SearchRoles != null &&
                prop.SearchRoles.Count > 0)
            {
                //Check if object is allowed for this search role
                //If the SearchRoles list is blank, then the obj is available for all roles
                if (prop.GetSearchRole(roleType) == null)
                {
                    bAllowed = false;
                }
            }

            return bAllowed;
        }

        //private void LoadSearchCriteriaGroup(PresentationGroup group, Telerik.Web.UI.RadTreeNodeCollection items, int level)
        //{
        //    if (group.GroupItems.Count > 0 ||
        //        group.Groups.Count > 0)
        //    {

        //        //Use RoleType to determine if we show the object/group
        //        RoleType roleType = RoleType.Undefined;
        //        if (HttpContext.Current.Session[SearchSessionKeys.sUserType] != null)
        //        {
        //            roleType = ConvertRole((string)HttpContext.Current.Session[SearchSessionKeys.sUserType]);
        //        }

        //        //Group contains something, create the menu item
        //        PresentationObject obj = group.AssociatedObject;
        //        if (obj != null &&
        //            IsObjAllowedForRole(obj, roleType))
        //        {
        //            if (level != 0)
        //            {
        //                string displayName = GetResourceString(obj.ResourceID, obj.DisplayName);
        //                Telerik.Web.UI.RadTreeNode newItem = new Telerik.Web.UI.RadTreeNode(displayName, obj.ID);
        //                newItem.ImageUrl = "~/images/folder.png";
        //                newItem.ToolTip = displayName;
        //                items.Add(newItem);
        //                items = newItem.Nodes;
        //            }


        //            //Load sub-groups
        //            foreach (PresentationGroup subGroup in group.Groups)
        //            {
        //                LoadSearchCriteriaGroup(subGroup, items, level + 1);
        //            }

        //            //Load group items
        //            foreach (PresentationGroupItem item in group.GroupItems)
        //            {
        //                if (item.IsVisible)
        //                {
        //                    PresentationObject itemObj = item.AssociatedObject;
        //                    if (itemObj != null &&
        //                        IsObjAllowedForRole(itemObj, roleType))
        //                    {
        //                        string objName = GetResourceString(itemObj.ResourceID, itemObj.DisplayName);

        //                        Telerik.Web.UI.RadTreeNode leafItem = new Telerik.Web.UI.RadTreeNode(objName, itemObj.ID);
        //                        leafItem.ImageUrl = "~/images/folder.png";
        //                        leafItem.ToolTip = objName;
        //                        items.Add(leafItem);

        //                        AddSearchCriteriaMenuItemsHelper(itemObj, leafItem.Nodes);
        //                    }
        //                }
        //            }

        //            if (level != 0)
        //            {
        //                //Load items for the group object
        //                AddSearchCriteriaMenuItemsHelper(obj, items);
        //            }
        //        }
        //    }
        //}

        //private void AddSearchCriteriaMenuItemsHelper(PresentationObject obj, Telerik.Web.UI.RadTreeNodeCollection items)
        //{

        //    RoleType roleType = RoleType.Undefined;
        //    if (HttpContext.Current.Session[SearchSessionKeys.sUserType] != null &&
        //        HttpContext.Current.Session[SearchSessionKeys.currObjectCriteriaViewId] != null)
        //    {
        //        roleType = ConvertRole((string)HttpContext.Current.Session[SearchSessionKeys.sUserType]);

        //        if (obj != null)
        //        {
        //            //Load Search Criteria Views for object (For now just load the first CriteriaView, indexed at 0)
        //            if (obj.CriteriaViews != null &&
        //                obj.CriteriaViews.Count > 0)
        //            {
        //                PresentationCriterionView view = obj.CriteriaViews[(int)(HttpContext.Current.Session[SearchSessionKeys.currObjectCriteriaViewId])];

        //                //Load Criteria list into Add Criteria Menu
        //                if (view.CriterionList != null)
        //                {
        //                    //Sort the list based on the Criterion index
        //                    view.CriterionList.Sort();

        //                    foreach (PresentationCriteria criteria in view.CriterionList)
        //                    {
        //                        PresentationProperty prop = criteria.AssociatedProperty;

        //                        if (prop != null &&
        //                            prop.IsSearchable &&
        //                            IsPropAllowedForRole(prop, roleType))
        //                        {
        //                            //Add the property
        //                            string displayName = GetResourceString(criteria.ResourceId, prop.DisplayName);

        //                            string propTag = GetTagFromProp(prop);

        //                            Telerik.Web.UI.RadTreeNode newItem = new Telerik.Web.UI.RadTreeNode(displayName, propTag);
        //                            items.Add(newItem);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //Convert search type to a Presentation RoleType
        public RoleType ConvertRole(String inputType)
        {
            RoleType role = RoleType.Undefined;

            if (0 == (String.Compare(inputType, SearchTypeKeys.Administrator, true)))
            {
                role = RoleType.Administrator;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.Owner, true)))
            {
                role = RoleType.Owner;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.Contributor, true)))
            {
                role = RoleType.Contributor;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.ReadAll, true)))
            {
                role = RoleType.ReadAll;
            }
            else if (0 == (String.Compare(inputType, SearchTypeKeys.ACL, true)))
            {
                role = RoleType.ACL;
            }

            return role;
        }

        #endregion

        #region Enumerations

        public enum PresentationViewType
        {
            Search,
            Result,
            Preview,
            Undefined
        }
        #endregion

    }

}

