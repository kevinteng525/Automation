using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

using Saber.Common;

namespace Saber.S1CommonAPILib
{
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1AddressRuleFilteringType
    {
        [Description("exRuleFltrNone")]
        None = 0,
        [Description("exRuleFltrAddress")]
        Address = 1,
        [Description("exRuleFltrAddressExactly")]
        DirectlyAddress = 3,
        [Description("exRuleFltrDomain")]
        Domain = 4,
        [Description("exRuleFltrMetaData")]
        MetaData_TBD = 6,
        [Description("exRuleFltrCatchDiscarded")]
        CopyMessagesNotMatchAnyRuleTo = 7,
        [Description("exRuleFltrKeyword")]
        Keyword = 9,
    }
    [TypeConverter(typeof(EnumToStringUsingDescription))]
    public enum S1AddressRuleFieldType
    {
        [Description("exRuleFieldNone")]
        None = 0,
        [Description("exRuleFieldRecipient")]
        To = 1,
        [Description("exRuleFieldSender")]
        From = 2,
        [Description("exRuleFieldSenderOrRecipient")]
        ToOrFrom = 3,
        [Description("exRuleFieldOwner")]
        OwnedBy = 4,
        [Description("exRuleFieldSubject")]
        WithSpecificWordsInSubject = 8,
        [Description("exRuleFieldAll")]
        All = 31,
    }
    public class S1AddressFilteringRuleCondition : IS1Object
    {
        public S1AddressRuleFilteringType FilterType { get; set; }
        public S1AddressRuleFieldType FieldType { get; set; }
        public List<S1DataSource> PeopleOrDistributionList { get; set; }
        public List<String> DomainOrSpecificWords { get; set; }
        public S1AddressFilteringRuleCondition()
        {
            this.PeopleOrDistributionList = new List<S1DataSource>();
            this.DomainOrSpecificWords = new List<String>();
        }

        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("The file not found:" + filePath);
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(XElement element)
        {
            String filtertype = XMLHelper.GetElementValue(element, "filtertype");
            String fieldtype = XMLHelper.GetElementValue(element, "fieldtype");
            XElement peopleordistributionlist = XMLHelper.GetElement(element, "peopleordistributionlist");
            List<S1DataSource> dataSourceList = new List<S1DataSource>();
            if (null != peopleordistributionlist)
            {                
                foreach (XElement e in peopleordistributionlist.Elements())
                {
                    S1DataSource source = new S1DataSource();
                    source.DeserializeFromXElement(e);
                    dataSourceList.Add(source);
                }
            }
            if (dataSourceList.Count > 0)
            {
                this.PeopleOrDistributionList = dataSourceList;
            }
            String domainorspecificwords = XMLHelper.GetElementValue(element, "domainorspecificwords");
            List<String> domainOrWordList = new List<String>();
            if (!String.IsNullOrEmpty( domainorspecificwords))
            {
                string[] domainOrWords = domainorspecificwords.Split(',');
                foreach (String d in domainOrWords)
                {
                    domainOrWordList.Add(d);
                }
            }
            if (domainOrWordList.Count > 0)
            {
                this.DomainOrSpecificWords = domainOrWordList;
            }
            if (!String.IsNullOrEmpty(filtertype))
            {
                if (filtertype.ToLower().Equals("address"))
                {
                    this.FilterType = S1AddressRuleFilteringType.Address;
                   
                }
                else if (filtertype.ToLower().Equals("directlyaddress"))
                {
                    this.FilterType = S1AddressRuleFilteringType.DirectlyAddress;
                   
                }
                else if (filtertype.ToLower().Equals("domain"))
                {
                    this.FilterType = S1AddressRuleFilteringType.Domain;
                  
                }
                else if (filtertype.ToLower().Equals("metadata"))
                {
                    //this.FilterType = S1AddressRuleFilteringType.MetaData_TBD;
                    throw new Exception("The filter type is not supportted yet.");
                }
                else if (filtertype.ToLower().Equals("copymessagesnotmatchanyruleto"))
                {
                    this.FilterType = S1AddressRuleFilteringType.CopyMessagesNotMatchAnyRuleTo;
                }
                else
                {
                    throw new Exception("The filter type is not valid.");
                }

            }
            else
            {
                throw new Exception("The filter type is needed!");
            }

            if (!String.IsNullOrEmpty(fieldtype))
            {
                switch (fieldtype.ToLower())
                {
                    case "to":
                        this.FieldType = S1AddressRuleFieldType.To;
                        break;
                    case "from":
                        this.FieldType = S1AddressRuleFieldType.From;
                        break;
                    case "toorfrom":
                        this.FieldType = S1AddressRuleFieldType.ToOrFrom;
                        break;
                    case "ownedby":
                        this.FieldType = S1AddressRuleFieldType.OwnedBy;
                        break;
                    case "withspecificwordsinsubject":
                        this.FieldType = S1AddressRuleFieldType.WithSpecificWordsInSubject;
                        break;
                    case "all":
                        this.FieldType = S1AddressRuleFieldType.All;
                        break;
                    default:
                        throw new Exception("The field type is not valid.");
                }
            }
            else
            {
                throw new Exception("Please specify the field type.");
            }
            return true;
        }
    }
    public class S1AddressFilteringRule : IS1Object
    {
        public String TargetMappedFolder { get; set; }
        public String Name { get; set; }
        public List<S1AddressFilteringRuleCondition> Conditions { get; set; }
        public S1AddressFilteringRule()
        {
            this.Conditions = new List<S1AddressFilteringRuleCondition>();
        }

        public bool DeserializeFromXMLFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Can not find the file specified.");
            }
            XDocument document = XDocument.Load(filePath);
            return DeserializeFromXElement(document.Root);
        }

        public bool DeserializeFromXElement(System.Xml.Linq.XElement element)
        {
            String name = XMLHelper.GetElementValue(element, "name");
            if (!String.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
            String targetmappedfolder = XMLHelper.GetElementValue(element, "targetmappedfolder");
            if (!String.IsNullOrEmpty(targetmappedfolder))
            {
                this.TargetMappedFolder = targetmappedfolder;
            }
            else
            {
                throw new Exception("The target mapped folder is required.");
            }
            XElement conditions = XMLHelper.GetElement(element, "addressfilteringruleconditions");
            List<S1AddressFilteringRuleCondition> conditionsList = new List<S1AddressFilteringRuleCondition>();
            if (null != conditions)
            {
                foreach (XElement condition in conditions.Elements())
                {
                    S1AddressFilteringRuleCondition c = new S1AddressFilteringRuleCondition();
                    c.DeserializeFromXElement(condition);
                    conditionsList.Add(c);
                }
            }
            else
            {
                throw new Exception("At least one condition should be specified.");
            }
            if (conditionsList.Count > 0)
            {
                this.Conditions = conditionsList;
            }
            else
            {
                throw new Exception("At least one condition should be specified.");
            }
            return true;
        }
    }
}
