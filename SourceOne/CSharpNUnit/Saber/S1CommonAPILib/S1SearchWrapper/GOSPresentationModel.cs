#region Copyright & License
  ///////////////////////////////////////////////////////////////////////////////////////
 //		Copyright © 1998 - 2009 EMC Corporation. All rights reserved.
 //		This software contains the intellectual property of EMC Corporation
 //		or is licensed to EMC Corporation from third parties. Use of this software
 //		and the intellectual property contained therein is expressly limited to
 //		the terms and conditions of the License Agreement under which it is
 //		provided by or on behalf of EMC.
 //						  EMC Corporation,
 //					      176 South St.,
 //					   Hopkinton, MA  01748.
 ///////////////////////////////////////////////////////////////////////////////////////

 #endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
//using GOSPresentationCommon;
using System.ComponentModel;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
//using GOSPresentationCommon.Extensibility;
using System.Xml.Schema;
using System.Reflection;
using System.Diagnostics;
//using GOSPresentationCommon.Resources;

namespace Saber.S1CommonAPILib.S1SearchWrapper
{
    public class XML_Constants
    {
        //Property Constants
        internal const string NPM_OBJECT_ID = "npmObjectId";
        internal const string NPM_PROP_ID = "npmPropId";
        internal const string DISPLAY_NAME = "displayName";
        internal const string RESOURCE_ID = "resourceId";
        internal const string FIELD_DISPLAY_TYPE = "fieldDisplayType";
        internal const string MAX_LENGTH = "maxLength";
        internal const string MAX_VALUE = "maxValue";
        internal const string MIN_VALUE = "minValue";
        internal const string DATA_TYPE = "dataType";
        internal const string SEARCH_OPERATIONS = "SearchOperations";
        internal const string ENUMERATIONS = "Enumerations";
        internal const string ENUM = "Enum";
        internal const string SEARCH_OPERATION = "SearchOperation";
        internal const string ROLES = "Roles";
        internal const string ROLE = "Role";
        internal const string ROLE_ID = "roleId";
        internal const string OPERATION = "operation";
        internal const string PRES_PROP_ID = "presPropId";
        internal const string PRES_PROP_ALIASES = "PropAliases";
        internal const string PRES_PROP_ALIAS = "PropAlias";

        //Role Types
        internal const string ROLE_ADMIN = "Administrator";
        internal const string ROLE_OWNER = "Owner";
        internal const string ROLE_CONTRIB = "Contributor";
        internal const string ROLE_READ_ALL = "ReadAll";
        internal const string ROLE_ACL = "ACL";

        //NPM Attr Constants
        internal const string NPMID = "NPMID";
        internal const string SRCH = "Srch";
        internal const string HIT = "Hit";
        internal const string NAME = "Name";
        internal const string TYPE = "Type";

        //Search Operations
        internal const string BEGINS_WITH = "BEGINS_WITH";
        internal const string CONTAINS = "CONTAINS";
        internal const string DOES_NOT_CONTAIN = "DOES_NOT_CONTAIN";
        internal const string ENDS_WITH = "ENDS_WITH";
        internal const string EQUALS = "EQUALS";
        internal const string GREATER_EQUAL = "GREATER_EQUAL";
        internal const string GREATER_THAN = "GREATER_THAN";
        internal const string LESS_EQUAL = "LESS_EQUAL";
        internal const string LESS_THAN = "LESS_THAN";
        internal const string NOT_EQUAL = "NOT_EQUAL";

        //XPATHs
        internal const string SEARCH_OP_PATH = "SearchOperations/SearchOperation";
        internal const string PROP_PATH = "Presentation.Properties/Prop";
        internal const string ENUM_PATH = "Enumerations/Enum";
        internal const string OBJECT_PATH = "Presentation.Objects/Object";
        internal const string CRITERION_VIEW_PATH = "Views/CriterionViews/CriterionView";
        internal const string RESULT_VIEW_PATH = "Views/ResultViews/ResultView";
        internal const string PREVIEW_VIEW_PATH = "Views/PreviewViews/PreviewView";
        internal const string PROP_ALIAS_PATH = "PropAliases/PropAlias";
        internal const string GROUP_PATH = "Presentation.Groups/Group";
        internal const string ROLE_PATH = "Roles/Role";

        internal const string EXTENSION_PROP_PATH = "Presentation.Extensions/Properties/Prop";
        internal const string EXTENSION_PROP_SEARCH_OP_PATH = "Presentation.Extensions/Properties/SearchOperation";
        internal const string EXTENSION_PROP_SEARCH_ENUM_PATH = "Presentation.Extensions/Properties/Enum";
        internal const string EXTENSION_PROP_SEARCH_ROLE_PATH = "Presentation.Extensions/Properties/Role";

        internal const string EXTENSION_OBJECT_PATH = "Presentation.Extensions/Objects/Object";
        internal const string EXTENSION_OBJECTS_CRITERIA_PATH = "Presentation.Extensions/Objects/Criteria";
        internal const string EXTENSION_OBJECTS_COLUMN_PATH = "Presentation.Extensions/Objects/Column";
        internal const string EXTENSION_OBJECTS_RESULT_VIEW_PATH = "Presentation.Extensions/Objects/ResultView";
        internal const string EXTENSION_OBJECTS_CRITERION_VIEW_PATH = "Presentation.Extensions/Objects/CriterionView";
        internal const string EXTENSION_OBJECTS_PREVIEW_VIEW_PATH = "Presentation.Extensions/Objects/PreviewView";
        internal const string EXTENSION_OBJECTS_PREVIEW_ITEM_PATH = "Presentation.Extensions/Objects/PreviewItem";
        internal const string EXTENSION_GROUP_PATH = "Presentation.Extensions/Groups/Group";

        internal const string EXTENSION_PROPERTIES_PATH = "Presentation.Extensions/Properties";
        internal const string EXTENSION_OBJECTS_PATH = "Presentation.Extensions/Objects";

        //NPM XPATHS
        internal const string NPM_OBJECTS_PATH = "//Objects/Object[@NPMID='{0}']";
        internal const string NPM_PROPS_PATH = "Props/Prop[@NPMID='{0}']";
        internal const string NPM_COMMON_PROPS_PATH = "CommonProps/Prop[@NPMID='{0}']";

        //Extensions
        internal const string METADATA = "Metadata";
        internal const string META = "Meta";
        internal const string ACTION = "action";
        internal const string ACTION_EDIT = "Edit";
        internal const string ACTION_ADD = "Add";
        internal const string ACTION_REMOVE = "Remove";
        internal const string CRITERION_VIEW_ID = "criterionViewId";
        internal const string RESULT_VIEW_ID = "resultViewId";
        internal const string PREVIEW_VIEW_ID = "previewViewId";

        //Sort Direction
        internal const string ASC = "asc";
        internal const string DESC = "desc";

        //Root
        internal const string ROOT_IMAGE_PATH = "rootImagePath";
        internal const string VERSION = "Version";

        //NPM field data type
        internal const string NPM_DATATYPE_DATE = "date";
        internal const string NPM_DATATYPE_STRING = "string";
        internal const string NPM_DATATYPE_SEMISTR = "semistr";
        internal const string NPM_DATATYPE_NVLIST = "NVList";
        internal const string NPM_DATATYPE_ROUTE = "route";
        internal const string NPM_DATATYPE_BLOB = "blob";
        internal const string NPM_DATATYPE_INT32 = "int32";
        internal const string NPM_DATATYPE_INT64 = "int64";
        internal const string NPM_DATATYPE_BOOLEAN = "boolean";

        //Field display type
        internal const string FIELD_DISPLAY_BOOLEAN = "Boolean";
        internal const string FIELD_DISPLAY_DATETIME = "DateTime";
        internal const string FIELD_DISPLAY_ADDRESS = "Address";
        internal const string FIELD_DISPLAY_ENUMERATION = "Enumeration";
        internal const string FIELD_DISPLAY_NUMBER = "Number";
        internal const string FIELD_DISPLAY_TEXT = "Text";

        // Field Data Type
        internal const string FIELD_DATA_STRING = "String";
        internal const string FIELD_DATA_DATETIME = "DateTime";
        internal const string FIELD_DATA_INT32 = "Int32";
        internal const string FIELD_DATA_INT64 = "Int64";
        internal const string FIELD_DATA_BOOLEAN = "Boolean";

        //Common
        internal const string ID = "id";
        internal const string OBJECT_ID = "objectId";
        internal const string IMAGE_PATH = "imagePath";

        //Grouping
        internal const string ITEM = "Item";
        internal const string GROUP = "Group";

        //Columns
        internal const string IS_GROUP_BY = "isGroupBy";
        internal const string IS_GROUP_BY_SORTED = "isGroupBySorted";
        internal const string GROUP_BY_INDEX = "groupByIndex";
        internal const string GROUP_BY_SORT_DIR = "groupBySortDir";
        internal const string GROUP_BY_SORT_PRIORITY = "groupBySortPriority";
        internal const string IS_SORTABLE = "isSortable";
        internal const string IS_SORTED = "isSorted";
        internal const string RESULT_SORT_DIR = "resultSortDir";
        internal const string SORT_PRIORITY = "sortPriority";
        internal const string RESULT_INDEX = "resultIndex";
        internal const string IS_VISIBLE = "isVisible";
        internal const string COLUMNS = "Columns";
        internal const string COLUMN = "Column";
        internal const string IS_CAPTION_IMG_VISIBLE = "isCaptionImgVisible";
        internal const string IS_CAPTION_VISIBLE = "isCaptionVisible";

        //Preview
        internal const string XSLT_LOCATION = "xsltLocation";
        internal const string PREVIEW_INDEX = "previewIndex";
        internal const string PREVIEW_VIEWS = "PreviewViews";
        internal const string PREVIEW_VIEW = "PreviewView";
        internal const string PREVIEW_ITEM = "PreviewItem";

        //Criteria
        internal const string CRITERIA_INDEX = "criteriaIndex";
        internal const string IS_DEFAULT = "isDefault";
        internal const string CRITERION_VIEW = "CriterionView";
        internal const string CRITERIA = "Criteria";
        internal const string CRITERION_VIEWS = "CriterionViews";


        //Object
        internal const string OBJECT = "Object";
        internal const string REF_ID = "refId";

        //Views
        internal const string VIEWS = "Views";
        internal const string RESULT_VIEWS = "ResultViews";
        internal const string RESULT_VIEW = "ResultView";

    }

    public class GOSPresentationModel : IGOSPresentationModel
    {
        #region Private Data Members

        private List<PresentationProperty> presentationPropertyList;
        private List<PresentationObject> presentationObjectList;
        private List<PresentationGroup> presentationGroupList;
        private XmlDocument presentationXmlDoc;
        private XmlDocument npmXmlDoc;
        private string rootImagePath;
        private string name = "SourceOne Presentation NPM";
        private string version = "1.0";
        private bool isXmlValid = true;

        /// <summary>
        /// Event handler property for subscribing to information type logging
        /// </summary>
        public event LogInformation OnLogInformation;

        /// <summary>
        /// Event handler property for subscribing to verbose type logging
        /// </summary>
        public event LogVerbose OnLogVerbose;

        /// <summary>
        /// Event handler property for subscribing to exception type logging
        /// </summary>
        public event LogException OnLogException;

        #endregion

        #region CTOR

        public GOSPresentationModel()
        {
            presentationXmlDoc = new XmlDocument();
            npmXmlDoc = new XmlDocument();
            presentationGroupList = new List<PresentationGroup>();
            presentationObjectList = new List<PresentationObject>();
            presentationPropertyList = new List<PresentationProperty>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves the Presentation XMLDocument instance
        /// </summary>
        internal XmlDocument PresentationXmlDocument
        {
            get { return presentationXmlDoc; }
        }

        /// <summary>
        /// Retrieves the NPM XMLDocument instance
        /// </summary>
        internal XmlDocument NPMXmlDocument
        {
            get { return npmXmlDoc; }
        }

        /// <summary>
        /// Indexer to retrieve an instance of a PresentationProperty object given the NPM Object and Property IDs
        /// </summary>
        /// <param name="NPMPropID">NPM Property ID</param>
        /// <param name="NPMObjectID">NPM Object ID</param>
        /// <returns>PresentationProperty instance</returns>
        public PresentationProperty this[int NPMPropID, int NPMObjectID]
        {
            get
            {
                return PresentationPropertyList.Find(prop => prop.NPMPropID == NPMPropID && prop.NPMObjectID == NPMObjectID);
            }
        }

        /// <summary>
        /// Indexer to retrieve an instance of a PresentationProperty object given the NPM Property ID
        /// </summary>
        /// <param name="NPMPropID">NPM Property ID</param>
        /// <returns>PresentationProperty instance</returns>
        public PresentationProperty this[int NPMPropID]
        {
            get
            {
                return PresentationPropertyList.Find(prop => prop.NPMPropID == NPMPropID);
            }
        }

        /// <summary>
        /// Indexer to retrieve an instance of a PresentationObject object given the object ID
        /// </summary>
        /// <param name="ObjectID">ID of the PresentationObject to retrieve</param>
        /// <returns>PresentationObject instance</returns>
        public PresentationObject this[string ObjectID]
        {
            get
            {
                return PresentationObjectList.Find(presObj => presObj.ID == ObjectID);
            }
        }

        /// <summary>
        /// Gets/Sets the Root Image Path. This string will be pre-pended to any ImagePath values
        /// </summary>
        public string RootImagePath
        {
            get { return rootImagePath; }
            set { rootImagePath = value; }
        }

        /// <summary>
        /// Gets the name attribute of the presentation xml
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the version number of the presentation xml
        /// </summary>
        public string Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets the list of PresentationProperty objects belonging to the model
        /// </summary>
        public List<PresentationProperty> PresentationPropertyList
        {
            get { return presentationPropertyList; }
            internal set { this.presentationPropertyList = value; }
        }

        /// <summary>
        /// Gets the list of PresentationObject objects belonging to the model
        /// </summary>
        public List<PresentationObject> PresentationObjectList
        {
            get { return presentationObjectList; }
            internal set { this.presentationObjectList = value; }
        }

        /// <summary>
        /// Gets the list of PresentationGroup objects belonging to the model
        /// </summary>
        public List<PresentationGroup> PresentationGroupList
        {
            get { return presentationGroupList; }
            internal set { this.presentationGroupList = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fires the OnLogException event to all subscribers
        /// </summary>
        /// <param name="ex">The exception that was caught and logged</param>
        internal void FireOnLogException(Exception ex)
        {
            if (this.OnLogException != null)
            {
                this.OnLogException.Invoke(ex);
            }
        }

        internal string GetTraceMethodString()
        {
            return getDeclaringTypeAndMethod() + " entered";
        }

        internal string GetTraceMethodExitString()
        {
            return getDeclaringTypeAndMethod() + " exited";
        }

        /// <summary>
        /// A helper method to create a XML document object of the current object model structure
        /// </summary>
        /// <returns>The Xml Document object representing the current object model</returns>
        private XmlDocument GetXmlDocument()
        {
            const string rootElement = "ES1.NPM.Presentation";
            const string propDictionaryElement = "Presentation.Properties";
            const string objectsElement = "Presentation.Objects";
            const string groupsElement = "Presentation.Groups";
            const string rootImgAttribute = "rootImagePath";

            XmlDocument es1NpmPresentation = new XmlDocument();

            XmlDeclaration declaration = es1NpmPresentation.CreateXmlDeclaration("1.0", "UTF-16", string.Empty);
            es1NpmPresentation.AppendChild(declaration);

            XmlElement pNpmRootElement = es1NpmPresentation.CreateElement(rootElement);
            XmlElement pNpmPrpsElement = es1NpmPresentation.CreateElement(propDictionaryElement);
            XmlElement pNpmObjsElement = es1NpmPresentation.CreateElement(objectsElement);
            XmlElement pNpmGrpsElement = es1NpmPresentation.CreateElement(groupsElement);

            XmlAttribute versionAttr = es1NpmPresentation.CreateAttribute(XML_Constants.VERSION);
            versionAttr.Value = this.Version;
            pNpmRootElement.Attributes.Append(versionAttr);

            XmlAttribute nameAttr = es1NpmPresentation.CreateAttribute(XML_Constants.NAME);
            nameAttr.Value = this.Name;
            pNpmRootElement.Attributes.Append(nameAttr);

            XmlAttribute rootImgPathAttr = es1NpmPresentation.CreateAttribute(rootImgAttribute);
            rootImgPathAttr.Value = this.RootImagePath;
            pNpmRootElement.Attributes.Append(rootImgPathAttr);

            es1NpmPresentation.AppendChild(pNpmRootElement);

            foreach (IGOSToXml prop in this.PresentationPropertyList)
            {
                pNpmPrpsElement.AppendChild(prop.ToXml(es1NpmPresentation));
            }

            foreach (IGOSToXml obj in this.PresentationObjectList)
            {
                pNpmObjsElement.AppendChild(obj.ToXml(es1NpmPresentation));
            }

            foreach (IGOSToXml grp in this.PresentationGroupList)
            {
                pNpmGrpsElement.AppendChild(grp.ToXml(es1NpmPresentation));
            }

            pNpmRootElement.AppendChild(pNpmPrpsElement);
            pNpmRootElement.AppendChild(pNpmObjsElement);
            pNpmRootElement.AppendChild(pNpmGrpsElement);

            return es1NpmPresentation;
        }

        private string getDeclaringTypeAndMethod()
        {
            StackTrace stack = new StackTrace();

            // get the parent frame of this method, should be the method calling this method
            StackFrame frame = stack.GetFrame(2);

            // get the parent method
            MethodBase method = frame.GetMethod();

            // get the parameter list of the parent method
            ParameterInfo[] paramList = method.GetParameters();

            // get the methods name, this is used for tracing
            if (method.DeclaringType != null) return method.DeclaringType.Name + "::" + method.Name;
            return null;
        }

        /// <summary>
        /// Fires the OnLogInformation event to all subscribers
        /// </summary>
        /// <param name="message">The information message</param>
        internal void FireOnLogInformation(string message)
        {
            if (this.OnLogInformation != null)
            {
                this.OnLogInformation.Invoke(message);
            }
        }

        /// <summary>
        /// Fires the OnLogVerbose event to all subscribers
        /// </summary>
        /// <param name="message">The verbose message</param>
        internal void FireOnLogVerbose(string message)
        {
            if (this.OnLogVerbose != null)
            {
                this.OnLogVerbose.Invoke(message);
            }
        }

        /// <summary>
        /// Creates an XmlReader object with the IgnoreWhitespace and IgnoreComments properties set to true
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        internal XmlReader createXmlReaderWithSettings(string xml)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreComments = true;
            return XmlReader.Create(new StringReader(xml), readerSettings);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a PresentationProperty instance from the model using the NPM Property and NPM Object ID
        /// </summary>
        /// <param name="NPMPropID">NPM Property ID</param>
        /// <param name="NPMObjectID">NPM Object ID</param>
        /// <returns>PresentationProperty instance</returns>
        public PresentationProperty GetProperty(int NPMPropID, int NPMObjectID)
        {
            return this[NPMPropID, NPMObjectID];
        }

        /// <summary>
        /// Retrieves a PresentationProperty instance from the model using the NPM Property ID
        /// </summary>
        /// <param name="NPMPropID">NPM Property ID</param>
        /// <returns>PresentationProperty instance</returns>
        public PresentationProperty GetProperty(int NPMPropID)
        {
            return this[NPMPropID];
        }

        /// <summary>
        /// Retrieves a PresentationObject instance from the model using the Object ID
        /// </summary>
        /// <param name="ObjectID">ID of object to retrieve</param>
        /// <returns>PresentationObject instance</returns>
        public PresentationObject GetObject(string ObjectID)
        {
            return this[ObjectID];
        }

        /// <summary>
        /// Validates the presentation XML against a well-defined schema
        /// </summary>
        /// <returns></returns>
        public bool IsXmlValid()
        {
            FireOnLogInformation("Validating Presentation XML");

            Stream xsdFileStream = getResourceStream();
            XmlTextReader reader = new XmlTextReader(xsdFileStream);

            XmlSchema schema = XmlSchema.Read(reader, null);

            presentationXmlDoc.Schemas.Add(schema);

            ValidationEventHandler handler = new ValidationEventHandler(Validationhandler);

            presentationXmlDoc.Validate(handler);

            FireOnLogInformation(string.Format("Validation {0}", (isXmlValid) ? "Successfull" : "Failed"));

            return isXmlValid;
        }

        private Stream getResourceStream()
        {
            string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (string resourceName in manifestResourceNames)
            {
                if (resourceName.EndsWith("ES1.NPM.Presentation.xsd"))
                {
                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                }
            }

            return null;
        }

        public void Validationhandler(Object sender, ValidationEventArgs e)
        {
            isXmlValid = false;

            FireOnLogException(e.Exception);
        }

        /// <summary>
        /// Retrieves an instance of PresentationProperty.  A presentation only property does not exist in the NPM definition
        /// </summary>
        /// <param name="PresentationPropID">ID of the presentation only property</param>
        /// <returns>PresentationProperty instance</returns>
        public PresentationProperty GetPresentationOnlyProperty(string PresentationPropID)
        {
            return PresentationPropertyList.Find(delegate(PresentationProperty prop)
                    { return prop.PresentationPropId == PresentationPropID; });
        }

        public GroupingItem GetPresentationGroup(string ObjectID)
        {
            GroupingItem presGroup = PresentationGroupList[0] as GroupingItem;

            getPresentationGroup(ObjectID, ref presGroup);

            return presGroup;
        }

        private void getPresentationGroup(string objectId, ref GroupingItem GroupItem)
        {
            if (GroupItem.AssociatedObject.ID == objectId)
                return;
            else
            {
                foreach (PresentationGroupItem groupItem in ((PresentationGroup)GroupItem).GroupItems)
                {
                    if (groupItem.AssociatedObject.ID == objectId)
                    {
                        GroupItem = groupItem as GroupingItem;
                        return;
                    }
                }

                foreach (PresentationGroup presGroup in ((PresentationGroup)GroupItem).Groups)
                {
                    if (GroupItem.AssociatedObject.ID == objectId)
                        return;

                    GroupItem = presGroup as GroupingItem;
                    getPresentationGroup(objectId, ref GroupItem);
                }
            }
        }

        /// <summary>
        /// Returns true if the object is already contained in the object list, false otherwise
        /// </summary>
        /// <param name="o">The object to search for</param>
        /// <returns>True if the object is present, false otherwise</returns>
        public bool ContainsObject(PresentationObject obj)
        {
            return (obj != null && this[obj.ID] != null);
        }

        /// <summary>
        /// Returns true if the property is already contained in the property list, false otherwise
        /// </summary>
        /// <param name="p">The property to search for</param>
        /// <returns>True if the property is present, false otherwise</returns>
        public bool ContainsProperty(PresentationProperty prop)
        {
            return (prop != null && this[prop.NPMPropID] != null);
        }

        /// <summary>
        /// Generates a default presentation npm xml file based on the es1 npm xml file
        /// </summary>
        /// <param name="es1NpmXml">The npm xml string</param>
        /// <param name="enumMappings">The 'property name - to - enum string values' dictionary</param>
        public void LoadFromNpm(string es1NpmXml, Dictionary<string, string[]> enumMappings)
        {
            FireOnLogVerbose("Loading default object model from npm file");

            try
            {
                // load the npm xml string
                this.npmXmlDoc.Load(createXmlReaderWithSettings(es1NpmXml));

                this.FireOnLogInformation("NPM xml file loaded successfully");
            }
            catch (Exception ex)
            {
                this.FireOnLogException(new Exception("Could not load the NPM xml file", ex));

                return;
            }

            // get the list of common props
            XmlNodeList commonProps = this.npmXmlDoc.SelectNodes("//CommonProps/Prop");

            // for each common prop, create the presentation prop list
            if (commonProps != null)
            {
                foreach (XmlNode prop in commonProps)
                {
                    PresentationProperty p = new PresentationProperty(prop, -1, enumMappings);

                    this.PresentationPropertyList.Add(p);
                }
            }

            // get the list of objects
            XmlNodeList npmObjects = this.npmXmlDoc.SelectNodes("//Objects/Object");

            // for each object, add object specific properties to the presentation prop list
            if (npmObjects != null)
            {
                foreach (XmlNode obj in npmObjects)
                {
                    List<PresentationProperty> objProperties = new List<PresentationProperty>();

                    // parse the object id
                    int objectID = AttributeRetrievalHelper.GetIntValue(obj.Attributes, XML_Constants.NPMID);

                    // parse the object name
                    string objectName = AttributeRetrievalHelper.GetStringValue(obj.Attributes, XML_Constants.NAME);

                    // get the list of common properties in this object 
                    XmlNodeList objCommonProps = obj.SelectNodes("Props/Prop[@RefID]");

                    // for each common property in the object, need to xpath to the actual property and add it to the views
                    if (objCommonProps != null)
                    {
                        foreach (XmlNode objCommonProp in objCommonProps)
                        {
                            // parse the 'RefID' attribute
                            int propRefID = AttributeRetrievalHelper.GetIntValue(objCommonProp.Attributes, "RefID");

                            // get the common property from the model
                            PresentationProperty presentationProp = this.GetProperty(propRefID);

                            // add it to the list of object properties if found
                            if (presentationProp != null)
                            {
                                objProperties.Add(presentationProp);
                            }
                        }
                    }

                    // get the list of object specific props 
                    XmlNodeList objProps = obj.SelectNodes("Props/Prop[@NPMID]");

                    // for each object prop, add to the presentation prop list
                    if (objProps != null)
                    {
                        foreach (XmlNode objProp in objProps)
                        {
                            PresentationProperty presentationProp = new PresentationProperty(objProp, objectID, enumMappings);

                            this.PresentationPropertyList.Add(presentationProp);

                            objProperties.Add(presentationProp);
                        }
                    }
                    PresentationObject pnpmObject = new PresentationObject(obj, objProperties);

                    // add the new object to the presentation object model object list
                    this.PresentationObjectList.Add(pnpmObject);
                }
            }

            FireOnLogVerbose("NPM Load complete");
        }

        /// <summary>
        /// Creates the GOSPresentationModel wrapper object from the GOS presentation and NPM XML 
        /// Validates the pXml value against the ES1.NPM.Presentation.xsd schema
        /// </summary>
        /// <param name="es1NpmXml">NPM XML contents as a string</param>
        /// <param name="pXml">Presentation XML contents as a string</param>
        public void LoadFromPxml(string es1NpmXml, string pXml)
        {
            this.FireOnLogVerbose("Loading object model from existing presenation xml file");

            try
            {
                // load the npm xml string
                npmXmlDoc.Load(createXmlReaderWithSettings(es1NpmXml));

                this.FireOnLogInformation("NPM xml file loaded successfully");
            }
            catch (Exception ex)
            {
                this.FireOnLogException(new Exception("Could not load the NPM xml file", ex));

                return;
            }

            try
            {
                // load the presentation xml string
                presentationXmlDoc.Load(createXmlReaderWithSettings(pXml));

                this.FireOnLogInformation("Presentation xml file loaded successfully");
            }
            catch (Exception ex)
            {
                this.FireOnLogException(new Exception("Could not load the presentation xml file", ex));

                return;
            }

            if (!IsXmlValid())
            {
                throw new ArgumentException("The presentation XML argument is invalid");
            }

            // parse the rootImagePath attribute 
            if (presentationXmlDoc.DocumentElement != null)
            {
                this.rootImagePath = AttributeRetrievalHelper.GetStringValue(presentationXmlDoc.DocumentElement.Attributes, XML_Constants.ROOT_IMAGE_PATH);

                // parse the name attribute
                this.name = AttributeRetrievalHelper.GetStringValue(presentationXmlDoc.DocumentElement.Attributes, XML_Constants.NAME);

                // parse the version attribute
                this.version = AttributeRetrievalHelper.GetStringValue(presentationXmlDoc.DocumentElement.Attributes, XML_Constants.VERSION);
            }

            //instantiate the class builder factory
            GOSPresentationModelFactory presentationModelFactory = new GOSPresentationModelFactory(presentationXmlDoc, npmXmlDoc, this);

            //create the presentation property list
            PresentationPropertyList = presentationModelFactory.CreatePropertyList();

            //create the presentation object list
            PresentationObjectList = presentationModelFactory.CreateObjectList();

            //create the group hierarchy
            if (presentationXmlDoc.DocumentElement != null)
            {
                XmlNodeList groupNodeList = presentationXmlDoc.DocumentElement.SelectNodes(XML_Constants.GROUP_PATH);

                PresentationGroupList = presentationModelFactory.CreateGroupList(groupNodeList);
            }

            FireOnLogVerbose("Presentation XML Load complete");
        }

        ///// <summary>
        ///// Applies an extensibility to the base Presentation XML model
        ///// </summary>
        ///// <param name="ExtensibilityXML">The extensibility xml definition as a string</param>
        public void LoadExtensibility(string extXml)
        {
            // if the extensibility xml is not null or empty
            if (string.IsNullOrEmpty(extXml) == false)
            {
                //FireOnLogVerbose("Loading Extensibility XML");

                //// create the presentation ext object model
                //GOSPresentationExtensibility extensibility = new GOSPresentationExtensibility(extXml, this);

                //// apply the extension to the object model
                //extensibility.ApplyExtensibility();

                //FireOnLogVerbose("Extensibility load complete");
            }
        }

        /// <summary>
        /// Saves the object model back into an xml file on the disk
        /// </summary>
        /// <param name="fileName">The file name path to save the xml, executing directory is null or empty</param>
        public void Save(string fileName)
        {   
            // if the file name is null or empty, just save the file to the executing directory
            if (String.IsNullOrEmpty(fileName) == true)
            {
                fileName = "ES1.NPM.Presentation.xml";
            }
            
            // Get the xml document object and save to the disk
            this.GetXmlDocument().Save(fileName);
        }

        public void Update(string pXmlUpdated)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the xml string representation of the object model
        /// </summary>
        /// <returns>String representation of the presenation XML</returns>
        public string ToXml()
        {
            return this.GetXmlDocument().InnerXml; 
        }

        #endregion
    }

    #region Classes

    #region Factory
    internal class GOSPresentationModelFactory
    {
        XmlDocument presentationXmlDocument;
        XmlDocument npmXmlDocument;
        GOSPresentationModel presentationModel;
        string rootImagePath = "";

        internal GOSPresentationModelFactory(XmlDocument PresentationXmlDocument, XmlDocument NPMXmlDocument, GOSPresentationModel presModel)
        {
            this.presentationXmlDocument = PresentationXmlDocument;
            this.npmXmlDocument = NPMXmlDocument;
            this.presentationModel = presModel;
            this.rootImagePath = presentationModel.RootImagePath;
        }

        #region Properties Creation

        internal List<PresentationProperty> CreatePropertyList()
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationProperty> presentationPropertyList = new List<PresentationProperty>();

            if (presentationXmlDocument.DocumentElement != null)
            {
                XmlNodeList propertyList = presentationXmlDocument.DocumentElement.SelectNodes(XML_Constants.PROP_PATH);

                if (propertyList != null)
                {
                    foreach (XmlNode propertyNode in propertyList)
                    {
                        presentationPropertyList.Add(CreateProperty(propertyNode, npmXmlDocument, rootImagePath));
                    }
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationPropertyList;
        }

        internal PresentationProperty CreateProperty(XmlNode propertyNode, XmlDocument npmXmlDocument, string rootImagePath)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationProperty presProperty = new PresentationProperty();

            bool objectIDExists = true;

            if (propertyNode != null)
            {
                if (propertyNode.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
                {
                    presProperty.NPMObjectID = AttributeRetrievalHelper.GetIntValue(propertyNode.Attributes, XML_Constants.NPM_OBJECT_ID);
                }
                else
                {
                    objectIDExists = false;
                }

                presProperty.NPMPropID = AttributeRetrievalHelper.GetIntValue(propertyNode.Attributes, XML_Constants.NPM_PROP_ID);

                presProperty.DisplayName = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.DISPLAY_NAME);
                presProperty.ResourceID = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.RESOURCE_ID);
                presProperty.FieldDisplayType = AttributeRetrievalHelper.GetFieldDisplayTypeValue(propertyNode.Attributes, XML_Constants.FIELD_DISPLAY_TYPE);
                presProperty.FieldDispTypeString = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.FIELD_DISPLAY_TYPE);
                presProperty.DataType = AttributeRetrievalHelper.GetFieldDataTypeValue(propertyNode.Attributes, XML_Constants.DATA_TYPE);
                presProperty.MaxLength = AttributeRetrievalHelper.GetIntValue(propertyNode.Attributes, XML_Constants.MAX_LENGTH);

                presProperty.MaxValue = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.MAX_VALUE);
                presProperty.MinValue = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.MIN_VALUE);

                if (propertyNode.Attributes[XML_Constants.IMAGE_PATH] != null)
                {
                    presProperty.OriginalImagePath = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.IMAGE_PATH);
                    presProperty.ImagePath = rootImagePath + presProperty.OriginalImagePath;
                }

                //pull in the npm extended properties
                if (objectIDExists)
                {
                    string objectXpath = string.Format(XML_Constants.NPM_OBJECTS_PATH, presProperty.NPMObjectID);
                    if (npmXmlDocument.DocumentElement != null)
                    {
                        XmlNode objectNode = npmXmlDocument.DocumentElement.SelectSingleNode(objectXpath);

                        if (objectNode != null)
                        {
                            string propXpath = string.Format(XML_Constants.NPM_PROPS_PATH, presProperty.NPMPropID);
                            XmlNode propNode = objectNode.SelectSingleNode(propXpath);

                            if (propNode != null)
                            {

                                presProperty.IsSearchable = AttributeRetrievalHelper.GetBoolValue(propNode.Attributes, XML_Constants.SRCH);

                                presProperty.IsSearchHit = AttributeRetrievalHelper.GetBoolValue(propNode.Attributes, XML_Constants.HIT);

                                presProperty.SearchName = AttributeRetrievalHelper.GetStringValue(propNode.Attributes, XML_Constants.NAME);

                                presProperty.PropType = AttributeRetrievalHelper.GetStringValue(propNode.Attributes, XML_Constants.TYPE);

                                presProperty.NPMDataType =  AttributeRetrievalHelper.GetNPMDataType(AttributeRetrievalHelper.GetStringValue(propNode.Attributes, XML_Constants.TYPE));
                            }
                        }
                    }
                }
                else
                {
                    //TODO: comment this 
                    string commonPropXPath = string.Format(XML_Constants.NPM_COMMON_PROPS_PATH, presProperty.NPMPropID);

                    XmlNode tempPropNode = null;

                    //if the npmObjectId nor the npmPropId is specified, then treat this property as a presentation only property
                    if (presProperty.NPMObjectID == -1 && presProperty.NPMPropID == -1)
                        tempPropNode = propertyNode;
                    else if (npmXmlDocument.DocumentElement != null) 
                        tempPropNode = npmXmlDocument.DocumentElement.SelectSingleNode(commonPropXPath);

                    if (tempPropNode != null)
                    {
                        presProperty.IsSearchable = AttributeRetrievalHelper.GetBoolValue(tempPropNode.Attributes, XML_Constants.SRCH);

                        presProperty.IsSearchHit = AttributeRetrievalHelper.GetBoolValue(tempPropNode.Attributes, XML_Constants.HIT);

                        presProperty.SearchName = AttributeRetrievalHelper.GetStringValue(tempPropNode.Attributes, XML_Constants.NAME);

                        presProperty.PropType = AttributeRetrievalHelper.GetStringValue(tempPropNode.Attributes, XML_Constants.TYPE);

                        presProperty.NPMDataType = AttributeRetrievalHelper.GetNPMDataType(AttributeRetrievalHelper.GetStringValue(tempPropNode.Attributes, XML_Constants.TYPE));
                    }
                }

                if (presProperty.DataType == FieldDataType.Undefined)
                {
                    presProperty.DataType = AttributeRetrievalHelper.GetFieldDataTypeFromNpmType(presProperty.PropType);
                }
            }

            //Presentation only property
            if (presProperty.NPMObjectID == -1 && presProperty.NPMPropID == -1)
            {
                presProperty.IsPresentationOnlyProperty = true;
                if (propertyNode != null) 
                    presProperty.PresentationPropId = AttributeRetrievalHelper.GetStringValue(propertyNode.Attributes, XML_Constants.PRES_PROP_ID);

                presentationModel.FireOnLogInformation(string.Format("Creating presentation-only PresentationProperty instance - presPropId = {0}", presProperty.PresentationPropId));
            }
            else
                presentationModel.FireOnLogInformation(string.Format("Creating PresentationProperty instance - npmPropId = {0}", presProperty.NPMPropID));


            //set the lists
            if (propertyNode != null)
            {
                presProperty.SearchOperationList = createSearchOperationList(propertyNode.SelectNodes(XML_Constants.SEARCH_OP_PATH));
                presProperty.SearchEnumerations = createSearchEnumerationList(propertyNode.SelectNodes(XML_Constants.ENUM_PATH), rootImagePath);
                presProperty.SearchRoles = createSearchRoleList(propertyNode.SelectNodes(XML_Constants.ROLE_PATH));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presProperty;
        }

        

        internal List<PresentationRole> createSearchRoleList(XmlNodeList searchRoles)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationRole> searchRoleList = new List<PresentationRole>();

            if (searchRoles != null && searchRoles.Count > 0)
            {
                PresentationRole presentationRole;
                foreach (XmlNode searchRole in searchRoles)
                {
                    presentationRole = CreatePresentationRole(searchRole);

                    searchRoleList.Add(presentationRole);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return searchRoleList;
        }

        internal PresentationRole CreatePresentationRole(XmlNode presentationRoleNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationRole presentationRole = new PresentationRole();

            if (presentationRoleNode.Attributes[XML_Constants.ROLE_ID] != null)
            {
                switch (presentationRoleNode.Attributes[XML_Constants.ROLE_ID].Value)
                {
                    case XML_Constants.ROLE_ADMIN:
                        presentationRole.RoleType = RoleType.Administrator;
                        break;
                    case XML_Constants.ROLE_OWNER:
                        presentationRole.RoleType = RoleType.Owner;
                        break;
                    case XML_Constants.ROLE_CONTRIB:
                        presentationRole.RoleType = RoleType.Contributor;
                        break;
                    case XML_Constants.ROLE_READ_ALL:
                        presentationRole.RoleType = RoleType.ReadAll;
                        break;
                    case XML_Constants.ROLE_ACL:
                        presentationRole.RoleType = RoleType.ACL;
                        break;

                }
                presentationModel.FireOnLogInformation(string.Format("Creating PresentationRole instance - RoleType = {0}", presentationRole.RoleType.ToString()));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationRole;
        }

        internal List<PresentationEnumeration> createSearchEnumerationList(XmlNodeList searchEnumerations, string rootImagePath)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationEnumeration> searchEnumerationList = new List<PresentationEnumeration>();

            if (searchEnumerations != null && searchEnumerations.Count > 0)
            {
                PresentationEnumeration presentationEnumeration;
                foreach (XmlNode searchEnumeration in searchEnumerations)
                {
                    presentationEnumeration = CreatePresentationEnumeration(searchEnumeration);

                    searchEnumerationList.Add(presentationEnumeration);
                }
            }
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return searchEnumerationList;
        }

        internal PresentationEnumeration CreatePresentationEnumeration(XmlNode presentationEnumerationNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationEnumeration presentationEnumeration = new PresentationEnumeration();

            presentationEnumeration = new PresentationEnumeration();
            presentationEnumeration.DisplayName = AttributeRetrievalHelper.GetStringValue(presentationEnumerationNode.Attributes, XML_Constants.DISPLAY_NAME);
            presentationEnumeration.ResourceID = AttributeRetrievalHelper.GetStringValue(presentationEnumerationNode.Attributes, XML_Constants.RESOURCE_ID);
            presentationEnumeration.SearchValue = presentationEnumerationNode.InnerText;

            if (presentationEnumerationNode.Attributes[XML_Constants.IMAGE_PATH] != null)
            {
                presentationEnumeration.OriginalImagePath = AttributeRetrievalHelper.GetStringValue(presentationEnumerationNode.Attributes, XML_Constants.IMAGE_PATH);
                presentationEnumeration.ImagePath = rootImagePath + presentationEnumeration.OriginalImagePath;
            }

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationEnumeration - SearchValue = {0}", presentationEnumeration.SearchValue.ToString()));

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());
            
            return presentationEnumeration;
        }

        internal List<PresentationSearchOperation> createSearchOperationList(XmlNodeList searchOperations)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationSearchOperation> searchOperationList = new List<PresentationSearchOperation>();

            if (searchOperations != null && searchOperations.Count > 0)
            {
                PresentationSearchOperation presentationSearchOperation;
                foreach (XmlNode searchOperation in searchOperations)
                {
                    presentationSearchOperation = CreateSearchOperation(searchOperation);

                    searchOperationList.Add(presentationSearchOperation);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return searchOperationList;
        }

        internal PresentationSearchOperation CreateSearchOperation(XmlNode searchOperationNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationSearchOperation searchOperation = new PresentationSearchOperation();

            searchOperation.OperationType =
                       AttributeRetrievalHelper.GetSearchOperationTypeValue(searchOperationNode.Attributes, XML_Constants.OPERATION);

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationSearchOperation - OperationType = {0}", searchOperation.OperationType.ToString()));

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return searchOperation;
        }
        

        #endregion

        #region Objects Creation
        internal List<PresentationObject> CreateObjectList()
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationObject> presentationObjectList = new List<PresentationObject>();
            List<XmlNode> presentationRefList = new List<XmlNode>();

            if (presentationXmlDocument.DocumentElement != null)
            {
                XmlNodeList objectList = presentationXmlDocument.DocumentElement.SelectNodes(XML_Constants.OBJECT_PATH);

                if (objectList != null)
                {
                    foreach (XmlNode objectNode in objectList)
                    {
                        if (objectNode.Attributes[XML_Constants.REF_ID] != null)
                            presentationRefList.Add(objectNode);
                        else
                            presentationObjectList.Add(CreateObject(objectNode, rootImagePath));
                    }
                }

                presentationModel.PresentationObjectList = presentationObjectList;

                if (presentationRefList.Count > 0)
                {
                    foreach (XmlNode refObjectNode in presentationRefList)
                    {
                        presentationObjectList.Add(CreateObjectFromRef(refObjectNode, rootImagePath));
                    }
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationObjectList;
        }

        internal PresentationObject CreateObjectFromRef(XmlNode objectNode, string rootImagePath)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            if (objectNode != null)
            {
                PresentationObject presObject = null;
                string refId;

                refId = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.REF_ID);
                PresentationObject pObject = presentationModel.GetObject(refId);

                if (pObject != null)
                {
                    presObject = new PresentationObject(pObject);

                    presObject.ID = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.ID);
                    presObject.ResourceID = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.RESOURCE_ID);
                    presObject.DisplayName = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.DISPLAY_NAME);
                    presObject.RefId = refId;

                    if (objectNode.Attributes[XML_Constants.IMAGE_PATH] != null)
                        presObject.ImagePath = rootImagePath + AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.IMAGE_PATH);

                    presentationModel.FireOnLogInformation(string.Format("Creating PresentationObject instace - Object ID = {0}", presObject.ID));

                }
                else
                    presentationModel.FireOnLogInformation("Unable to create this Object because the reference object does not exist");
                
                presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());
                
                return presObject;
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return null;
        }

        internal PresentationObject CreateObject(XmlNode objectNode, string rootImagePath)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationObject presentationObject = new PresentationObject();

            if (objectNode != null)
            {
                presentationObject.DisplayName = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.DISPLAY_NAME);
                presentationObject.ResourceID = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.RESOURCE_ID);
                presentationObject.ID = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.ID);

                if (objectNode.Attributes[XML_Constants.IMAGE_PATH] != null)
                {
                    presentationObject.OriginalImagePath = AttributeRetrievalHelper.GetStringValue(objectNode.Attributes, XML_Constants.IMAGE_PATH);
                    presentationObject.ImagePath = rootImagePath + presentationObject.OriginalImagePath;
                }
            }

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationObject instance - Object ID = {0}", presentationObject.ID));

            //set the lists

            if (objectNode != null)
            {
                presentationObject.CriteriaViews = createCriteronViewList(objectNode.SelectNodes(XML_Constants.CRITERION_VIEW_PATH));

                presentationObject.ResultViews = createResultViewList(objectNode.SelectNodes(XML_Constants.RESULT_VIEW_PATH));

                presentationObject.PreviewViews = createPreviewViewList(objectNode.SelectNodes(XML_Constants.PREVIEW_VIEW_PATH));

                presentationObject.PropertyAliases = createPropertyAliasList(objectNode.SelectNodes(XML_Constants.PROP_ALIAS_PATH));

                presentationObject.Roles = createSearchRoleList(objectNode.SelectNodes(XML_Constants.ROLE_PATH));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationObject;
        }

        private List<PresentationPropertyAlias> createPropertyAliasList(XmlNodeList propertyAliasNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationPropertyAlias> propertyAliasList = new List<PresentationPropertyAlias>();

            if (propertyAliasNodeList != null && propertyAliasNodeList.Count > 0)
            {
                PresentationPropertyAlias propertyAlias;

                foreach (XmlNode propAliasNode in propertyAliasNodeList)
                {
                    propertyAlias = CreatePropertyAlias(propAliasNode);

                    propertyAliasList.Add(propertyAlias);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return propertyAliasList;
        }

        private List<PresentationResultView> createResultViewList(XmlNodeList resultViewXmlNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationResultView> resultViewList = new List<PresentationResultView>();

            if (resultViewXmlNodeList != null && resultViewXmlNodeList.Count > 0)
            {
                PresentationResultView presentationResultView;

                foreach (XmlNode resultViewNode in resultViewXmlNodeList)
                {
                    presentationResultView = CreatePresentationResultView(resultViewNode);

                    resultViewList.Add(presentationResultView);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return resultViewList;
        }

        internal PresentationResultView CreatePresentationResultView(XmlNode ResultViewNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationResultView presentationResultView = new PresentationResultView();

            presentationResultView.DisplayName = AttributeRetrievalHelper.GetStringValue(ResultViewNode.Attributes, XML_Constants.DISPLAY_NAME);
            presentationResultView.ResourceID = AttributeRetrievalHelper.GetStringValue(ResultViewNode.Attributes, XML_Constants.RESOURCE_ID);

            presentationResultView.ID = AttributeRetrievalHelper.GetIntValue(ResultViewNode.Attributes, XML_Constants.ID);

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationResultView instance - ID = {0}", presentationResultView.ID));

            XmlNodeList columnListNode = ResultViewNode.SelectNodes(XML_Constants.COLUMNS);
            if (columnListNode != null && columnListNode.Count > 0)
                presentationResultView.ColumnList = createResultViewColumnList(columnListNode[0].ChildNodes);

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationResultView;
        }

        private List<PresentationColumn> createResultViewColumnList(XmlNodeList presentationColumnsXmlNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationColumn> presentationColumnList = new List<PresentationColumn>();

            if (presentationColumnsXmlNodeList != null && presentationColumnsXmlNodeList.Count > 0)
            {
                PresentationColumn presentationColumn;
                foreach (XmlNode presentationColumnNode in presentationColumnsXmlNodeList)
                {
                    presentationColumn = CreatePresentationColumn(presentationColumnNode);

                    if (presentationColumn != null)
                        presentationColumnList.Add(presentationColumn);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationColumnList;
        }

        internal PresentationPropertyAlias CreatePropertyAlias(XmlNode propAliasNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationPropertyAlias propAlias = new PresentationPropertyAlias();

            if (propAliasNode != null)
            {
                propAlias.NPMPropID = AttributeRetrievalHelper.GetIntValue(propAliasNode.Attributes, XML_Constants.NPM_PROP_ID);

                if (propAliasNode.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
                {
                    propAlias.NPMObjectID = AttributeRetrievalHelper.GetIntValue(propAliasNode.Attributes, XML_Constants.NPM_OBJECT_ID);
                    propAlias.AssociatedNPMProperty = presentationModel.GetProperty(propAlias.NPMPropID, propAlias.NPMObjectID);
                }
                else 
                {
                    propAlias.AssociatedNPMProperty = presentationModel.GetProperty(propAlias.NPMPropID);
                }

                if (propAliasNode.Attributes[XML_Constants.PRES_PROP_ID] != null)
                {
                    propAlias.PresentationOnlyPropID = AttributeRetrievalHelper.GetStringValue(propAliasNode.Attributes, XML_Constants.PRES_PROP_ID);
                    propAlias.AssociatedPresentationOnlyProperty = presentationModel.GetPresentationOnlyProperty(propAlias.PresentationOnlyPropID);
                }

                presentationModel.FireOnLogInformation(string.Format("Creating PresentationPropertyAlias - ID = {0}", propAlias.NPMPropID));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return propAlias;
        }

        internal PresentationColumn CreatePresentationColumn(XmlNode presentationColumnNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationColumn presentationColumn = new PresentationColumn();

            if (presentationColumnNode != null)
            {
                presentationColumn.NPMPropID = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.NPM_PROP_ID);
                if (presentationColumnNode.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
                {
                    presentationColumn.NPMObjectID = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.NPM_OBJECT_ID);
                    presentationColumn.AssociatedProperty = presentationModel.GetProperty(presentationColumn.NPMPropID, presentationColumn.NPMObjectID);
                }
                else if (presentationColumn.NPMPropID != -1)
                {
                    presentationColumn.AssociatedProperty = presentationModel.GetProperty(presentationColumn.NPMPropID);
                }
                else if (presentationColumnNode.Attributes[XML_Constants.PRES_PROP_ID] != null)
                {
                    presentationColumn.PresPropID = AttributeRetrievalHelper.GetStringValue(presentationColumnNode.Attributes, XML_Constants.PRES_PROP_ID);
                    presentationColumn.AssociatedProperty = presentationModel.GetPresentationOnlyProperty(presentationColumn.PresPropID);
                }

                presentationColumn.IsGroupBy = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_GROUP_BY);
                presentationColumn.IsGroupBySorted = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_GROUP_BY_SORTED);
                presentationColumn.GroupByIndex = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.GROUP_BY_INDEX);
                presentationColumn.GroupBySortDirection = AttributeRetrievalHelper.GetSortDirectionEnumValue(presentationColumnNode.Attributes, XML_Constants.GROUP_BY_SORT_DIR);
                presentationColumn.GroupBySortPriority = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.GROUP_BY_SORT_PRIORITY);

                presentationColumn.IsSortable = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_SORTABLE);
                presentationColumn.IsSorted = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_SORTED);
                presentationColumn.ResultSortDirection = AttributeRetrievalHelper.GetSortDirectionEnumValue(presentationColumnNode.Attributes, XML_Constants.RESULT_SORT_DIR);
                presentationColumn.SortPriority = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.SORT_PRIORITY);
                presentationColumn.ResultIndex = AttributeRetrievalHelper.GetIntValue(presentationColumnNode.Attributes, XML_Constants.RESULT_INDEX);
                presentationColumn.IsVisible = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_VISIBLE);
                presentationColumn.ResourceId = AttributeRetrievalHelper.GetStringValue(presentationColumnNode.Attributes, XML_Constants.RESOURCE_ID);

                if (presentationColumnNode.Attributes[XML_Constants.IS_CAPTION_IMG_VISIBLE] != null)
                    presentationColumn.IsCaptionImgVisible = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_CAPTION_IMG_VISIBLE);

                if (presentationColumnNode.Attributes[XML_Constants.IS_CAPTION_VISIBLE] != null)
                    presentationColumn.IsCaptionVisible = AttributeRetrievalHelper.GetBoolValue(presentationColumnNode.Attributes, XML_Constants.IS_CAPTION_VISIBLE);
            }

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationColumn instance - npmPropId = {0}", presentationColumn.NPMPropID));

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationColumn;
        }

        private List<PresentationPreviewView> createPreviewViewList(XmlNodeList previewViewXmlNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationPreviewView> previewViewList = new List<PresentationPreviewView>();

            if (previewViewXmlNodeList != null && previewViewXmlNodeList.Count > 0)
            {
                PresentationPreviewView presentationPreviewView;
                foreach (XmlNode previewViewNode in previewViewXmlNodeList)
                {
                    presentationPreviewView = CreatePresentationPreviewView(previewViewNode);

                    previewViewList.Add(presentationPreviewView);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return previewViewList;
        }

        internal PresentationPreviewView CreatePresentationPreviewView(XmlNode presentationPreviewViewNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationPreviewView presentationPreviewView = new PresentationPreviewView();

            presentationPreviewView.ID = AttributeRetrievalHelper.GetIntValue(presentationPreviewViewNode.Attributes, XML_Constants.ID);
            presentationPreviewView.XsltLocation = AttributeRetrievalHelper.GetStringValue(presentationPreviewViewNode.Attributes, XML_Constants.XSLT_LOCATION);

            presentationPreviewView.PreviewItems = createPreviewItemList(presentationPreviewViewNode.ChildNodes);

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationPreviewView instance - ID = {0}", presentationPreviewView.ID));

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationPreviewView;
        }

        private List<PresentationPreviewItem> createPreviewItemList(XmlNodeList previewItemXmlNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationPreviewItem> previewItemList = new List<PresentationPreviewItem>();

            if (previewItemXmlNodeList != null && previewItemXmlNodeList.Count > 0)
            {
                PresentationPreviewItem presentationPreviewItem;
                foreach (XmlNode previewItemNode in previewItemXmlNodeList)
                {
                    presentationPreviewItem = CreatePresentationPreviewItem(previewItemNode);

                    previewItemList.Add(presentationPreviewItem);
                }
            }
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return previewItemList;
        }

        internal PresentationPreviewItem CreatePresentationPreviewItem(XmlNode presentationPreviewItemNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationPreviewItem presentationPreviewItem = new PresentationPreviewItem();

            presentationPreviewItem.NPMPropID = AttributeRetrievalHelper.GetIntValue(presentationPreviewItemNode.Attributes, XML_Constants.NPM_PROP_ID);
            if (presentationPreviewItemNode.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
            {
                presentationPreviewItem.NPMObjectID = AttributeRetrievalHelper.GetIntValue(presentationPreviewItemNode.Attributes, XML_Constants.NPM_OBJECT_ID);
                presentationPreviewItem.AssociatedProperty = presentationModel.GetProperty(presentationPreviewItem.NPMPropID, presentationPreviewItem.NPMObjectID);
            }
            else if (presentationPreviewItem.NPMPropID != -1)
            {
                presentationPreviewItem.AssociatedProperty = presentationModel.GetProperty(presentationPreviewItem.NPMPropID);
            }
            else if (presentationPreviewItemNode.Attributes[XML_Constants.PRES_PROP_ID] != null)
            {
                presentationPreviewItem.AssociatedProperty = presentationModel.GetPresentationOnlyProperty(AttributeRetrievalHelper.GetStringValue(presentationPreviewItemNode.Attributes, XML_Constants.PRES_PROP_ID));
            }

            presentationModel.FireOnLogInformation(string.Format("Creating PresentationPreviewItem instance - npmPropId = {0}", presentationPreviewItem.NPMPropID));

            presentationPreviewItem.PreviewIndex = AttributeRetrievalHelper.GetIntValue(presentationPreviewItemNode.Attributes, XML_Constants.PREVIEW_INDEX);

            presentationPreviewItem.ResourceId = AttributeRetrievalHelper.GetStringValue(presentationPreviewItemNode.Attributes, XML_Constants.RESOURCE_ID);

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationPreviewItem;
        }

        private List<PresentationCriterionView> createCriteronViewList(XmlNodeList criterionXmlNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationCriterionView> criterionViewList = new List<PresentationCriterionView>();

            if (criterionXmlNodeList != null && criterionXmlNodeList.Count > 0)
            {
                PresentationCriterionView criterionView;

                foreach (XmlNode criterionViewNode in criterionXmlNodeList)
                {
                    criterionView = CreateCriterionView(criterionViewNode);

                    criterionViewList.Add(criterionView);
                }
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return criterionViewList;
        }

        internal PresentationCriterionView CreateCriterionView(XmlNode presentationCriteriaViewNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationCriterionView criterionView = new PresentationCriterionView();

            criterionView.ID = AttributeRetrievalHelper.GetIntValue(presentationCriteriaViewNode.Attributes, XML_Constants.ID);

            List<PresentationCriteria> criteriaList = new List<PresentationCriteria>();
            criterionView.CriterionList = criteriaList;

            foreach (XmlNode criteriaNode in presentationCriteriaViewNode.ChildNodes)
            {
                criteriaList.Add(CreatePresentationCriteria(criteriaNode));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return criterionView;
        }

        internal PresentationCriteria CreatePresentationCriteria(XmlNode presentationCriteriaNode)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationCriteria presentationCriteria = new PresentationCriteria();

            if (presentationCriteriaNode != null)
            {
                presentationCriteria.NPMPropID = AttributeRetrievalHelper.GetIntValue(presentationCriteriaNode.Attributes, XML_Constants.NPM_PROP_ID);
                if (presentationCriteriaNode.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
                {
                    presentationCriteria.NPMObjectID = AttributeRetrievalHelper.GetIntValue(presentationCriteriaNode.Attributes, XML_Constants.NPM_OBJECT_ID);
                    presentationCriteria.AssociatedProperty = presentationModel.GetProperty(presentationCriteria.NPMPropID, presentationCriteria.NPMObjectID);
                }
                else if (presentationCriteria.NPMPropID != -1)
                {
                    presentationCriteria.AssociatedProperty = presentationModel.GetProperty(presentationCriteria.NPMPropID);
                }
                else if (presentationCriteriaNode.Attributes[XML_Constants.PRES_PROP_ID] != null)
                {
                    presentationCriteria.PresPropID = AttributeRetrievalHelper.GetStringValue(presentationCriteriaNode.Attributes, XML_Constants.PRES_PROP_ID);
                    presentationCriteria.AssociatedProperty = presentationModel.GetPresentationOnlyProperty(presentationCriteria.PresPropID);
                }

                presentationCriteria.CriteriaIndex = AttributeRetrievalHelper.GetIntValue(presentationCriteriaNode.Attributes, XML_Constants.CRITERIA_INDEX);

                presentationCriteria.IsDefault = AttributeRetrievalHelper.GetBoolValue(presentationCriteriaNode.Attributes, XML_Constants.IS_DEFAULT);

                presentationCriteria.ResourceId = AttributeRetrievalHelper.GetStringValue(presentationCriteriaNode.Attributes, XML_Constants.RESOURCE_ID);

                presentationModel.FireOnLogInformation(string.Format("Creating PresentationCriteria instance - npmPropId = {0}", presentationCriteria.NPMPropID));
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return presentationCriteria;
        }

        #endregion

        #region Groups Creation

        /// <summary>
        /// Creates a list of composite PresentationGroup objects
        /// </summary>
        /// <param name="presentationGroupNodeList">The XML Node list of presentation group node(s)
        /// Most likely one group, at the root level, will exist</param>
        /// <returns>List of PresentationGroup objects</returns>
        internal List<PresentationGroup> CreateGroupList(XmlNodeList presentationGroupNodeList)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            List<PresentationGroup> groupList = new List<PresentationGroup>();

            foreach (XmlNode groupNode in presentationGroupNodeList)
            {
                PresentationGroup group = null;
                createGroupInstance(groupNode, ref group);
                groupList.Add(group);
            }

            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());

            return groupList;
        }

        /// <summary>
        /// Recursive method for creating a root group n levels deep
        /// </summary>
        /// <param name="groupNode"></param>
        /// <param name="group"></param>
        private void createGroupInstance(XmlNode groupNode, ref PresentationGroup group)
        {
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodString());

            PresentationGroup groupTemp = new PresentationGroup();

            if (group != null)
                group.Groups.Add(groupTemp);
            else
                group = groupTemp;

            groupTemp.ObjectID = AttributeRetrievalHelper.GetStringValue(groupNode.Attributes, XML_Constants.OBJECT_ID);
          
            groupTemp.IsVisible = (groupNode.Attributes[XML_Constants.IS_VISIBLE] == null)?
                true : AttributeRetrievalHelper.GetBoolValue(groupNode.Attributes, XML_Constants.IS_VISIBLE);

            PresentationObject associatedOjbect = presentationModel.GetObject(groupTemp.ObjectID);

            if (associatedOjbect != null)
            {
                groupTemp.AssociatedObject = associatedOjbect;
                groupTemp.AssociatedObject.AssociatedGroup = groupTemp;
            }

            var xmlNodeList = groupNode.SelectNodes(XML_Constants.ITEM);
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                PresentationGroupItem presentationGroupItem;
                groupTemp.GroupItems = new List<PresentationGroupItem>();
                var groupItemNodes = groupNode.SelectNodes(XML_Constants.ITEM);
                if (groupItemNodes != null)
                    foreach (XmlNode groupItemNode in groupItemNodes)
                    {
                        presentationGroupItem = new PresentationGroupItem();
                        presentationGroupItem.ObjectID = AttributeRetrievalHelper.GetStringValue(groupItemNode.Attributes, XML_Constants.OBJECT_ID);
                   
                        presentationGroupItem.IsVisible = (groupItemNode.Attributes[XML_Constants.IS_VISIBLE] == null) ?
                                                                                                                           true : AttributeRetrievalHelper.GetBoolValue(groupItemNode.Attributes, XML_Constants.IS_VISIBLE);
                    
                        presentationGroupItem.AssociatedObject = presentationModel.GetObject(presentationGroupItem.ObjectID);
                        presentationGroupItem.AssociatedObject.AssociatedGroup = presentationGroupItem;
                        groupTemp.GroupItems.Add(presentationGroupItem);
                    }
            }

            var selectNodes = groupNode.SelectNodes(XML_Constants.GROUP);
            if (selectNodes != null && selectNodes.Count > 0)
            {
                var innerGroupXmlNodes = groupNode.SelectNodes(XML_Constants.GROUP);
                if (innerGroupXmlNodes != null)
                    foreach (XmlNode innerGroupXmlNode in innerGroupXmlNodes)
                    {
                        createGroupInstance(innerGroupXmlNode, ref groupTemp);
                    }
            }
            presentationModel.FireOnLogVerbose(presentationModel.GetTraceMethodExitString());
        }

        #endregion


    }

    #endregion

    #region Static Helpers

    internal static class Cloner
    {
        public static object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }
    }



    public static class AttributeRetrievalHelper
    {
        /// <summary>
        /// Generates the default resource ID for the given elements with resource IDs based on it's display name
        /// </summary>
        /// <param name="name">The display name</param>
        /// <returns>The resource ID based on the name</returns>
        public static string GetResourceIdFromName(string name, ResourceIdType type)
        {
            // the format is "RESOURCE_<>_[], where [] is the name in upper case, with the spaces replaced with underscore characters, and <> is the resourceId type
            return string.Format("RESOURCE_{0}_{1}", type.ToString().ToUpper(), name.ToUpper().Replace(' ', '_'));
        }

        public static bool GetBoolValue(XmlAttributeCollection AttrCollection, string Key)
        {
            bool boolValue = false;
            if(AttrCollection[Key] != null)
                Boolean.TryParse(AttrCollection[Key].Value, out boolValue);
            return boolValue;
        }

        public static int GetIntValue(XmlAttributeCollection AttrCollection, string Key)
        {
            int result = -1;
            
            if(Int32.TryParse(GetStringValue(AttrCollection,Key), out result))
                return result;
            else
                return -1;
        }

        public static string GetStringValue(XmlAttributeCollection AttrCollection, string Key)
        {
            string returnValue = null;

            if (AttrCollection[Key] != null)
            {
                returnValue = AttrCollection[Key].Value;
            }

            return returnValue;
        }

        public static SortDirection GetSortDirectionEnumValue(XmlAttributeCollection AttrCollection, string Key)
        {
            if (AttrCollection[Key] != null)
            {
                string groupByString = AttrCollection[Key].Value;
                return (groupByString == XML_Constants.ASC) ? SortDirection.Asc : SortDirection.Desc;
            }
            else
                //default to asc
                return SortDirection.Asc;
        }

        /// <summary>
        /// Returns a FieldDataType value based on the NPM Type Attribute of a property
        /// </summary>
        /// <param name="npmType">The value of the Type Attribute</param>
        /// <returns>FieldDataType</returns>
        public static FieldDataType GetFieldDataTypeFromNpmType(string npmType)
        {
            switch (npmType)
            {
                case XML_Constants.NPM_DATATYPE_DATE:
                    return FieldDataType.DateTime;
                case XML_Constants.NPM_DATATYPE_STRING:
                case XML_Constants.NPM_DATATYPE_SEMISTR:
                case XML_Constants.NPM_DATATYPE_NVLIST:
                case XML_Constants.NPM_DATATYPE_ROUTE:
                case XML_Constants.NPM_DATATYPE_BLOB:
                    return FieldDataType.String;
                case XML_Constants.NPM_DATATYPE_INT32:
                    return FieldDataType.Int32;
                case XML_Constants.NPM_DATATYPE_INT64:
                    return FieldDataType.Int64;
                case XML_Constants.NPM_DATATYPE_BOOLEAN:
                    return FieldDataType.Boolean;
                default:
                    return FieldDataType.String;
            }
        }

        public static NPMDataType GetNPMDataType(string npmTypeString)
        {
            NPMDataType npmDataType = NPMDataType.Undefined;

            switch (npmTypeString)
            {
                case "blob":
                    return NPMDataType.Blob;
                case "string":
                    return NPMDataType.String;
                case "semistr":
                    return NPMDataType.SemiStr;
                case "int32":
                    return NPMDataType.Int32;
                case "int64":
                    return NPMDataType.Int64;
                case "boolean":
                    return NPMDataType.Boolean;
                case "date":
                    return NPMDataType.Date;
                case "route":
                    return NPMDataType.Route;
            }

            return npmDataType;
        }

        /// <summary>
        /// Returns a FieldDisplayType value based on the NPM Type Attribute of a property
        /// </summary>
        /// <param name="npmType">The value of the Type Attribute</param>
        /// <returns>FieldDisplayType</returns>
        public static FieldDispType GetFieldDisplayTypeFromNpmType(string npmType)
        {
            switch (npmType)
            {
                case XML_Constants.NPM_DATATYPE_DATE:
                    return FieldDispType.DateTime;
                case XML_Constants.NPM_DATATYPE_STRING:
                case XML_Constants.NPM_DATATYPE_SEMISTR:
                case XML_Constants.NPM_DATATYPE_NVLIST:
                case XML_Constants.NPM_DATATYPE_BLOB:
                case XML_Constants.NPM_DATATYPE_INT32:
                case XML_Constants.NPM_DATATYPE_INT64:
                    return FieldDispType.Text;
                case XML_Constants.NPM_DATATYPE_ROUTE:
                    return FieldDispType.Address;
                case XML_Constants.NPM_DATATYPE_BOOLEAN:
                    return FieldDispType.Boolean;
                default:
                    return FieldDispType.Text;
            }
        }

        public static FieldDispType GetFieldDisplayTypeValue(XmlAttributeCollection AttrCollection, string Key)
        {
            if (AttrCollection[Key] != null)
            {
                switch (AttrCollection[Key].Value)
                {
                    case XML_Constants.FIELD_DISPLAY_BOOLEAN:
                        return FieldDispType.Boolean;
                    case XML_Constants.FIELD_DISPLAY_DATETIME:
                        return FieldDispType.DateTime;
                    case XML_Constants.FIELD_DISPLAY_ADDRESS:
                        return FieldDispType.Address;
                    case XML_Constants.FIELD_DISPLAY_ENUMERATION:
                        return FieldDispType.Enumeration;
                    case XML_Constants.FIELD_DISPLAY_NUMBER:
                        return FieldDispType.Number;
                    case XML_Constants.FIELD_DISPLAY_TEXT:
                        return FieldDispType.Text;
                    default:
                        return FieldDispType.Undefined;
                    //TODO: Log this as information
                    //throw new ArgumentException(string.Format("Key provided {'0'} does not exist in the attribute collection", Key));
                }
            }
            else
                return FieldDispType.Undefined;
        }

        public static FieldDataType GetFieldDataTypeValue(XmlAttributeCollection AttrCollection, string Key)
        {
            if (AttrCollection[Key] != null)
            {
                switch (AttrCollection[Key].Value)
                {
                    case XML_Constants.FIELD_DATA_BOOLEAN:
                        return FieldDataType.Boolean;
                    case XML_Constants.FIELD_DATA_DATETIME:
                        return FieldDataType.DateTime;
                    case XML_Constants.FIELD_DATA_INT32:
                        return FieldDataType.Int32;
                    case XML_Constants.FIELD_DATA_INT64:
                        return FieldDataType.Int64;
                    case XML_Constants.FIELD_DATA_STRING:
                        return FieldDataType.String;
                    default:
                        return FieldDataType.Undefined;
                    //TODO: Log this as information
                    //throw new ArgumentException(string.Format("Key provided {'0'} does not exist in the attribute collection", Key));
                }
            }
            else
                return FieldDataType.Undefined;
        }

        public static SearchOperationType GetSearchOperationTypeValue(XmlAttributeCollection AttrCollection, string Key)
        {
            if (AttrCollection[Key] != null)
            {
                switch (AttrCollection[Key].Value)
                {
                    case XML_Constants.BEGINS_WITH:
                        return SearchOperationType.BEGINS_WITH;
                    case XML_Constants.CONTAINS:
                        return SearchOperationType.CONTAINS;
                    case XML_Constants.DOES_NOT_CONTAIN:
                        return SearchOperationType.DOES_NOT_CONTAIN;
                    case XML_Constants.ENDS_WITH:
                        return SearchOperationType.ENDS_WITH;
                    case XML_Constants.EQUALS:
                        return SearchOperationType.EQUALS;
                    case XML_Constants.GREATER_EQUAL:
                        return SearchOperationType.GREATER_EQUAL;
                    case XML_Constants.GREATER_THAN:
                        return SearchOperationType.GREATER_THAN;
                    case XML_Constants.LESS_EQUAL:
                        return SearchOperationType.LESS_EQUAL;
                    case XML_Constants.LESS_THAN:
                        return SearchOperationType.LESS_THAN;
                    case XML_Constants.NOT_EQUAL:
                        return SearchOperationType.NOT_EQUAL;
                    default:
                        return SearchOperationType.Undefined;
                    //TODO: Log this as information
                    //throw new ArgumentException(string.Format("Key provided {'0'} does not exist in the attribute collection", Key));
                }
            }
            else
                return SearchOperationType.Undefined;
        }
    }

    #endregion

    /// <summary>
    /// Base class for all Presentation model concrete classes
    /// Contains metadata collection
    /// </summary>
    [Serializable]
    public abstract class PresentationXmlElement
    {
        private NameValueCollection metaDataCollection;

        public NameValueCollection MetaDataCollection
        {
            get { return metaDataCollection; }
            set { metaDataCollection = value; }
        }

        private bool _isExtensibility = false;

        public bool IsExtensibility
        {
            get
            {
                return _isExtensibility;
            }
        }

        public PresentationXmlElement()
        {
            metaDataCollection = new NameValueCollection();
        }

        internal void SetIsExtensibility(bool isExtensibility)
        {
            this._isExtensibility = isExtensibility;
        }
    }

    [Serializable]
    public abstract class PresentationPropertyDerived : PresentationXmlElement, IGOSPropertyDescriptor
    {
        private PresentationProperty associatedProperty;
        protected string resourceId;

        public string ResourceId
        {
            get
            {
                if (!string.IsNullOrEmpty(resourceId))
                    return resourceId;
                else
                    return associatedProperty.ResourceID;
            }
            set { resourceId = value; }
        }

        #region IGOSPropertyDescriptor Members

        public PresentationProperty AssociatedProperty
        {
            get { return associatedProperty; }
            set { associatedProperty = value; }
        }
        #endregion
    }

    /// <summary>
    /// Base class for Group type objects
    /// </summary>
    public abstract class GroupingItem : PresentationXmlElement
    {
        private PresentationObject associatedObject;

        /// <summary>
        /// The PresentationObject that this object is associated with.
        /// </summary>
        public PresentationObject AssociatedObject
        {
            get { return associatedObject; }
            set { associatedObject = value; }
        }
    }

    /// <summary>
    /// Base class for all displayable items
    /// </summary>
    [Serializable]
    public abstract class DisplayItem : PresentationXmlElement
    {
        private string displayName;
        private string  resourceID;

        /// <summary>
        /// The displayName attribute value - the fallback display name (used primarily to display something if the localization fails)
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        /// <summary>
        /// The resourceId attribute value - suggested resource ID for pulling localized text to display
        /// </summary>
        public string ResourceID 
        {
            get { return resourceID; }
            set { resourceID = value; }
        }

        //public string GetLocalizedResourceString()
        //{
        //    if (!string.IsNullOrEmpty(resourceID))
        //        return GOSPresentationStrings.ResourceManager.GetString(resourceID);
        //    else
        //        return displayName;
        //}
    }

    /// <summary>
    /// Class to encapsulate the 'Prop' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationProperty : DisplayItem, IGOSToXml, IComparable
    {
        private const string XmlNodeName = "Prop";

        //private data memmbers
        private int npmObjectID = -1;
        private int npmPropID;
        private int maxLength;
        private int suggestedIndex;
        private string minValue;
        private string maxValue;
        private FieldDispType fieldDisplayType;
        private FieldDataType fieldDataType;
        private NPMDataType npmDataType;

        private string fieldDispTypeString;

        private string searchName;
        private string propType;
        private string origImagePath, imagePath;
        private string presentationPropId;
        private bool isSearchable;
        private bool isSearchHit;
        private bool isPresentationOnlyProp;
        private List<PresentationSearchOperation> searchOperationList;
        private List<PresentationEnumeration> searchEnumerations;
        private List<PresentationRole> searchRoles;

        /// <summary>
        /// Constructs an empty presentation property
        /// </summary>
        public PresentationProperty()
        {
            this.searchOperationList = new List<PresentationSearchOperation>();
            this.searchEnumerations = new List<PresentationEnumeration>();
            this.searchRoles = new List<PresentationRole>();
        }

        /// <summary>
        /// Constructs a default presentation property based on a npm property (either from common, or from an object)
        /// </summary>
        /// <param name="npmProp">The npm property xml blob</param>
        /// <param name="objectId">The object ID from the npm (-1 if the npm prop is a common prop)</param>
        public PresentationProperty(XmlNode npmProp, int objectId, Dictionary<string, string[]> enumMapping) : this()
        {
            // set property values from the npm property
            this.NPMPropID = AttributeRetrievalHelper.GetIntValue(npmProp.Attributes, XML_Constants.NPMID);
            this.DisplayName = AttributeRetrievalHelper.GetStringValue(npmProp.Attributes, XML_Constants.NAME);
            this.IsSearchable = AttributeRetrievalHelper.GetBoolValue(npmProp.Attributes, XML_Constants.SRCH);
            this.IsSearchHit = AttributeRetrievalHelper.GetBoolValue(npmProp.Attributes, XML_Constants.HIT);

            // set the object ID
            this.NPMObjectID = objectId;

            // get the resource ID from the display name
            this.ResourceID = AttributeRetrievalHelper.GetResourceIdFromName(this.DisplayName, ResourceIdType.Prop);

            // get the npm type attribute
            this.PropType = AttributeRetrievalHelper.GetStringValue(npmProp.Attributes, XML_Constants.TYPE);

            NPMDataType = AttributeRetrievalHelper.GetNPMDataType(AttributeRetrievalHelper.GetStringValue(npmProp.Attributes, XML_Constants.TYPE));

            // get the Field Data Type from the NPM type attribute
            this.DataType = AttributeRetrievalHelper.GetFieldDataTypeFromNpmType(this.PropType);

            // get the Field Display Type from the NPM type attribute
            this.FieldDisplayType = AttributeRetrievalHelper.GetFieldDisplayTypeFromNpmType(this.PropType);

            this.fieldDispTypeString = AttributeRetrievalHelper.GetStringValue(npmProp.Attributes, XML_Constants.FIELD_DISPLAY_TYPE);

            // if the objectId is '0' then this code is being called from the constructor based on a pres prop (so no need for default search operations)
            if (objectId != 0)
            {
                if (enumMapping != null)
                {
                    this.AddDefaultEnumerations(enumMapping);
                }

                // add the default search operations
                this.AddDefaultSearchOperations();
            }
        }

        /// <summary>
        /// Constructs a presentation property based on an existing presentation property
        /// </summary>
        /// <param name="pXmlProp">The presentation property xml blob</param>
        /// <param name="npmProp">The npm property xml blob</param>
        public PresentationProperty(XmlNode pXmlProp, XmlNode npmProp) : this(npmProp, 0, null)
        {
            // get the property values from the presentation property
            this.ImagePath = AttributeRetrievalHelper.GetStringValue(pXmlProp.Attributes, XML_Constants.IMAGE_PATH);
            this.MaxLength = AttributeRetrievalHelper.GetIntValue(pXmlProp.Attributes, XML_Constants.MAX_LENGTH);
            this.MaxValue = AttributeRetrievalHelper.GetStringValue(pXmlProp.Attributes, XML_Constants.MAX_VALUE);
            this.MinValue = AttributeRetrievalHelper.GetStringValue(pXmlProp.Attributes, XML_Constants.MIN_VALUE);

            // try to get the npmObjectId from the presentation property
            if (pXmlProp.Attributes[XML_Constants.NPM_OBJECT_ID] != null)
            {
                this.NPMObjectID = AttributeRetrievalHelper.GetIntValue(pXmlProp.Attributes, XML_Constants.NPM_OBJECT_ID);
            }
            else
            {
                this.NPMObjectID = -1;
            }

            // get the list of enumerations
            XmlNodeList enumerations = pXmlProp.SelectNodes(XML_Constants.ENUM_PATH);

            // add the enumerations to the property
            this.AddEnumerations(enumerations);

            // get the list of search operations
            XmlNodeList searchOperations = pXmlProp.SelectNodes(XML_Constants.SEARCH_OP_PATH);

            // add the search operations to the property
            this.AddSearchOperations(searchOperations);

            this.fieldDispTypeString = AttributeRetrievalHelper.GetStringValue(npmProp.Attributes, XML_Constants.FIELD_DISPLAY_TYPE);

            // get the list of roles
            XmlNodeList roles = pXmlProp.SelectNodes(XML_Constants.ROLE_PATH);

            // add the roles to the property
            this.AddRoles(roles);
        }

        /// <summary>
        /// The npmObjectId attribute value
        /// </summary>
        public int NPMObjectID
        {
            get { return npmObjectID; }
            set { npmObjectID = value; }
        }

        /// <summary>
        /// The npmPropId attribute value
        /// </summary>
        public int NPMPropID 
        {
            get { return npmPropID; }
            set { npmPropID = value; }
        }
        
        /// <summary>
        /// The presPropId attribute value - presentation only property if not null and IsPresentationOnlyProperty = true (does not exist in the NPM)
        /// </summary>
        public string PresentationPropId
        {
            get { return presentationPropId; }
            set { presentationPropId = value; }
        }

        /// <summary>
        /// The maxLength attribute value
        /// </summary>
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; }
        }

        /// <summary>
        /// The imagePath attribute value - this value is appended to the rootImagePath attribute located on the root element 
        /// </summary>
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        public string OriginalImagePath
        {
            set { origImagePath = value; }
            get { return origImagePath; }
        }

        /// <summary>
        /// The minValue attribute value
        /// </summary>
        public string MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        /// <summary>
        /// The maxValue attribute value
        /// </summary>
        public string MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        /// <summary>
        /// The fieldDisplayType attribute value - gets/sets a FieldDispType enum
        /// </summary>
        public FieldDispType FieldDisplayType
        {
            get { return fieldDisplayType; }
            set { fieldDisplayType = value; }
        }

        /// <summary>
        /// The FieldDispTypeString attribute value - gets/sets the string value, from the presxml, that represents the FieldDispType
        /// </summary>
        public string FieldDispTypeString
        {
            get { return fieldDispTypeString; }
            set { fieldDispTypeString = value; }
        }

        /// <summary>
        /// The dataType attribute value - gets/sets a FieldDataType enum
        /// </summary>
        public FieldDataType DataType 
        {
            get { return fieldDataType; }
            set { fieldDataType = value; }
        }

        /// <summary>
        /// The NPM datatype as defined in the ES
        /// </summary>
        public NPMDataType NPMDataType
        {
            get { return npmDataType; }
            set { npmDataType = value; }
        }

        /// <summary>
        /// If true, then this property instance is not defined in the NPM
        /// </summary>
        public bool IsPresentationOnlyProperty
        {
            get { return isPresentationOnlyProp; }
            set { isPresentationOnlyProp = value; }
        }

        //NPM PROPERTIES
        /// <summary>
        /// The Name attribute value from the NPM definition
        /// </summary>
        public string SearchName 
        {
            get { return searchName; }
            set { searchName = value; }
        }

        /// <summary>
        /// The Type attribute value from the NPM definition
        /// </summary>
        public string PropType
        {
            get { return propType; }
            set { propType = value; }
        }

        /// <summary>
        /// The Srch attribute value from the NPM definition
        /// </summary>
        public bool IsSearchable 
        {
            get { return isSearchable; }
            set { isSearchable = value; }
        }

        /// <summary>
        /// The Hit attribute value from the NPM definition
        /// </summary>
        public bool IsSearchHit 
        {
            get { return isSearchHit; }
            set { isSearchHit = value; }
        }

        public int SuggestedIndex
        {
            get { return suggestedIndex; }
            set { suggestedIndex = value; }
        }

        //Aggregates
        /// <summary>
        /// Gets a list of PresentationSearchOperation objects definded for this property. 
        /// </summary>
        public List<PresentationSearchOperation> SearchOperationList 
        {
            get { return searchOperationList; }
            internal set { searchOperationList = value; }
        }

        /// <summary>
        /// Gets a list of PresentationEnumeration objects defined for this property 
        /// </summary>
        public List<PresentationEnumeration> SearchEnumerations 
        {
            get { return searchEnumerations; }
            internal set { searchEnumerations = value; }
        }

        /// <summary>
        /// Gets a list of PresentationRole objects defined for this property
        /// </summary>
        public List<PresentationRole> SearchRoles
        {
            get { return searchRoles; }
            internal set { searchRoles = value; }
        }

        /// <summary>
        /// Retrieves a PresentationEnumeration instance given an enumeration value
        /// </summary>
        /// <param name="EnumerationValue">The enumeration value to search for e.g. "LOW"</param>
        /// <returns>PresentationEnumeration instance</returns>
        public PresentationEnumeration GetSearchEnumeration(string EnumerationValue)
        {
            return searchEnumerations.Find(delegate(PresentationEnumeration enumeration)
                    { return enumeration.SearchValue == EnumerationValue; });
        }
        
        /// <summary>
        /// Adds the default search operations to the property based on the property field display type
        /// </summary>
        private void AddDefaultSearchOperations()
        {
            switch (this.FieldDisplayType)
            {
                case FieldDispType.Address:
                    AddSearchOperation(SearchOperationType.CONTAINS);
                    AddSearchOperation(SearchOperationType.DOES_NOT_CONTAIN);
                    AddSearchOperation(SearchOperationType.BEGINS_WITH);
                    AddSearchOperation(SearchOperationType.ENDS_WITH);
                    AddSearchOperation(SearchOperationType.EQUALS);
                    break;
                case FieldDispType.Boolean:
                case FieldDispType.Enumeration:
                    AddSearchOperation(SearchOperationType.EQUALS);
                    break;
                case FieldDispType.DateTime:
                    AddSearchOperation(SearchOperationType.GREATER_EQUAL);
                    AddSearchOperation(SearchOperationType.LESS_THAN);
                    break;
                case FieldDispType.Text:
                    AddSearchOperation(SearchOperationType.CONTAINS);
                    AddSearchOperation(SearchOperationType.DOES_NOT_CONTAIN);
                    AddSearchOperation(SearchOperationType.BEGINS_WITH);
                    AddSearchOperation(SearchOperationType.ENDS_WITH);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds the search operations to the property
        /// </summary>
        /// <param name="searchOperations">The search operation list xml blob</param>
        private void AddSearchOperations(XmlNodeList searchOperations)
        {
            // for each search operation
            foreach (XmlNode searchOperation in searchOperations)
            {
                // add the search operation
                AddSearchOperation(AttributeRetrievalHelper.GetSearchOperationTypeValue(searchOperation.Attributes, XML_Constants.OPERATION));
            }
        }

        /// <summary>
        /// Adds a search operation to the list of search operations
        /// </summary>
        /// <param name="operation">The search operation type to add</param>
        private void AddSearchOperation(SearchOperationType operationType)
        {
            // create a new search operation and set the operation type
            PresentationSearchOperation searchOperation = new PresentationSearchOperation();
            searchOperation.OperationType = operationType;

            // add to the search operation list
            this.SearchOperationList.Add(searchOperation);
        }

        /// <summary>
        /// Adds the list of default enumerations to the property
        /// </summary>
        /// <param name="enumMapping">The dictionary of property display name - to - enum values</param>
        private void AddDefaultEnumerations(Dictionary<string, string[]> enumMapping)
        {
            // add the enums if the property name is a key in the enum mappings
            if (enumMapping.ContainsKey(this.DisplayName) == true)
            {
                // make sure to change the field display type to 'enumeration'
                this.FieldDisplayType = FieldDispType.Enumeration;

                // the enumeration value is now in the following format, <DISPLAY_NAME>_<VALUE>, need to split the string based on the underscore character
                foreach (string enumeration in enumMapping[this.DisplayName])
                {
                    string[] enumNameValue = enumeration.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    string displayName = string.Empty;
                    string resourceId = string.Empty;
                    string value = string.Empty;
                    string imgPath = string.Empty;

                    // ignore bad input from the app.config file, the array must be of length 2 (with no img path attr) or 3 with 
                    if (enumNameValue.Length == 2)
                    {
                        displayName = enumNameValue[0].ToLower();
                        displayName = Char.ToUpper(displayName[0]) + displayName.Substring(1);
                        resourceId = AttributeRetrievalHelper.GetResourceIdFromName((this.DisplayName.ToUpper() + "_" + displayName.ToUpper()), ResourceIdType.Enum);
                        value = enumNameValue[1];
                    }
                    else if (enumNameValue.Length == 3)
                    {
                        displayName = enumNameValue[0].ToLower();
                        displayName = Char.ToUpper(displayName[0]) + displayName.Substring(1);
                        resourceId = AttributeRetrievalHelper.GetResourceIdFromName((this.DisplayName.ToUpper() + "_" + displayName.ToUpper()), ResourceIdType.Enum);
                        value = enumNameValue[1];
                        imgPath = enumNameValue[2];
                    }

                    if (displayName.Length > 0)
                    {
                        this.AddEnumeration(resourceId, displayName, imgPath, value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the enumerations to the property
        /// </summary>
        /// <param name="enumerations">The enumerations xml blob</param>
        private void AddEnumerations(XmlNodeList enumerations)
        {
            if (enumerations.Count > 0)
            { 
                // make sure to change the field display type to 'enumeration'
                this.FieldDisplayType = FieldDispType.Enumeration;
            }

            foreach (XmlNode enumeration in enumerations)
            {
                string name = AttributeRetrievalHelper.GetStringValue(enumeration.Attributes, XML_Constants.DISPLAY_NAME);
                string resourceId = AttributeRetrievalHelper.GetStringValue(enumeration.Attributes, XML_Constants.RESOURCE_ID);
                string imagePath = AttributeRetrievalHelper.GetStringValue(enumeration.Attributes, XML_Constants.IMAGE_PATH);
                string value = enumeration.InnerText;

                this.AddEnumeration(resourceId, name, imagePath, value);
            }
        }

        /// <summary>
        /// Adds a search enumeration to the list of search enumerations
        /// </summary>
        /// <param name="resourceId">The resource ID of the enum</param>
        /// <param name="name">The display name of the enum</param>
        /// <param name="imagePath">The imagepath of the enum</param>
        /// <param name="value">The search value of the enum</param>
        private void AddEnumeration(string resourceId, string name, string imagePath, string value)
        {
            PresentationEnumeration enumeration = new PresentationEnumeration();
            enumeration.DisplayName = name;
            enumeration.OriginalImagePath = imagePath;
            enumeration.ResourceID = resourceId;
            enumeration.SearchValue = value;

            this.SearchEnumerations.Add(enumeration);
        }

        /// <summary>
        /// Adds the roles to the property
        /// </summary>
        /// <param name="roles"></param>
        private void AddRoles(XmlNodeList roles)
        {
            foreach (XmlNode role in roles)
            {
                if (role.Attributes[XML_Constants.ROLE_ID] != null)
                {
                    PresentationRole presentationRole = new PresentationRole();

                    switch (role.Attributes[XML_Constants.ROLE_ID].Value)
                    {
                        case XML_Constants.ROLE_ADMIN:
                            presentationRole.RoleType = RoleType.Administrator;
                            break;
                        case XML_Constants.ROLE_OWNER:
                            presentationRole.RoleType = RoleType.Owner;
                            break;
                        case XML_Constants.ROLE_CONTRIB:
                            presentationRole.RoleType = RoleType.Contributor;
                            break;
                        case XML_Constants.ROLE_READ_ALL:
                            presentationRole.RoleType = RoleType.ReadAll;
                            break;
                    }

                    this.SearchRoles.Add(presentationRole);
                }
            }
        }

        public PresentationSearchOperation GetSearchOperation(SearchOperationType SearchOpType)
        {
            return searchOperationList.Find(delegate(PresentationSearchOperation operation)
            { return operation.OperationType == SearchOpType; });
        }

        public PresentationRole GetSearchRole(RoleType RoleType)
        {
            return SearchRoles.Find(delegate(PresentationRole role)
            { return role.RoleType == RoleType; });
        }

        #region IGOSToXml Members

        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc">XMLDocument in which to create XmlElements from</param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement propElement = xmlDoc.CreateElement(XmlNodeName);

            if (this.IsPresentationOnlyProperty == false)
            {
                XmlAttribute npmPropIDAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_PROP_ID);
                npmPropIDAttr.Value = this.NPMPropID.ToString();
                propElement.Attributes.Append(npmPropIDAttr);

                if (this.NPMObjectID >= 0)
                {
                    XmlAttribute npmObjectIDAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_OBJECT_ID);
                    npmObjectIDAttr.Value = this.NPMObjectID.ToString();
                    propElement.Attributes.Append(npmObjectIDAttr);
                }
            }
            else
            {
                XmlAttribute presPropIDAttr = xmlDoc.CreateAttribute(XML_Constants.PRES_PROP_ID);
                presPropIDAttr.Value = this.PresentationPropId;
                propElement.Attributes.Append(presPropIDAttr);

                XmlAttribute searchableAttr = xmlDoc.CreateAttribute(XML_Constants.SRCH);
                searchableAttr.Value = this.IsSearchable.ToString().ToLower();
                propElement.Attributes.Append(searchableAttr);

                XmlAttribute srchHitAttr = xmlDoc.CreateAttribute(XML_Constants.HIT);
                srchHitAttr.Value = this.isSearchHit.ToString().ToLower();
                propElement.Attributes.Append(srchHitAttr);
            }

            XmlAttribute resourceIDAttr = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
            resourceIDAttr.Value = this.ResourceID;
            propElement.Attributes.Append(resourceIDAttr);

            XmlAttribute displayNameAttr = xmlDoc.CreateAttribute(XML_Constants.DISPLAY_NAME);
            displayNameAttr.Value = this.DisplayName;
            propElement.Attributes.Append(displayNameAttr);

            XmlAttribute fieldDisplayTypeAttr = xmlDoc.CreateAttribute(XML_Constants.FIELD_DISPLAY_TYPE);
            fieldDisplayTypeAttr.Value = this.FieldDisplayType.ToString();
            propElement.Attributes.Append(fieldDisplayTypeAttr);

            XmlAttribute dataTypeAttr = xmlDoc.CreateAttribute(XML_Constants.DATA_TYPE);
            dataTypeAttr.Value = this.DataType.ToString();
            propElement.Attributes.Append(dataTypeAttr);

            XmlAttribute maxLengthAttr = xmlDoc.CreateAttribute(XML_Constants.MAX_LENGTH);
            maxLengthAttr.Value = this.MaxLength.ToString();
            propElement.Attributes.Append(maxLengthAttr);

            XmlAttribute maxValueAttr = xmlDoc.CreateAttribute(XML_Constants.MAX_VALUE);
            maxValueAttr.Value = this.MaxValue;
            propElement.Attributes.Append(maxValueAttr);

            XmlAttribute minValueAttr = xmlDoc.CreateAttribute(XML_Constants.MIN_VALUE);
            minValueAttr.Value = this.MinValue;
            propElement.Attributes.Append(minValueAttr);

            XmlAttribute imgPathAttr = xmlDoc.CreateAttribute(XML_Constants.IMAGE_PATH);
            //imgPathAttr.Value = this.ImagePath;
            imgPathAttr.Value = this.OriginalImagePath;
            propElement.Attributes.Append(imgPathAttr);

            if (this.SearchOperationList != null && this.SearchOperationList.Count > 0)
            {
                XmlElement searchOpsElement = xmlDoc.CreateElement(XML_Constants.SEARCH_OPERATIONS);

                foreach (IGOSToXml operation in this.SearchOperationList)
                {
                    searchOpsElement.AppendChild(operation.ToXml(xmlDoc));
                }

                propElement.AppendChild(searchOpsElement);
            }

            if (this.SearchEnumerations != null && this.SearchEnumerations.Count > 0)
            {
                XmlElement enumsElement = xmlDoc.CreateElement(XML_Constants.ENUMERATIONS);

                foreach (IGOSToXml enumeration in this.SearchEnumerations)
                {
                    enumsElement.AppendChild(enumeration.ToXml(xmlDoc));
                }

                propElement.AppendChild(enumsElement);
            }

            if (this.SearchRoles != null && this.SearchRoles.Count > 0)
            {
                XmlElement rolesElement = xmlDoc.CreateElement(XML_Constants.ROLES);

                foreach (IGOSToXml role in this.SearchRoles)
                {
                    rolesElement.AppendChild(role.ToXml(xmlDoc));
                }

                propElement.AppendChild(rolesElement);
            }

            return propElement;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is PresentationProperty)
            {
                PresentationProperty otherPresentationProperty = (PresentationProperty)obj;
                return this.suggestedIndex.CompareTo(otherPresentationProperty.SuggestedIndex);
            }
            else
            {
                throw new ArgumentException("Object is not of type PresentationProperty");
            }
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'SearchOperation' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationSearchOperation : PresentationXmlElement, IGOSToXml
    {
        private SearchOperationType operationType;

        /// <summary>
        /// The 'operation' attribute value - returns a SearchOperationType enum value
        /// </summary>
        public SearchOperationType OperationType 
        {
            get { return operationType; }
            set { operationType = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement searchOpElement = xmlDoc.CreateElement(XML_Constants.SEARCH_OPERATION);

            XmlAttribute operationAttr = xmlDoc.CreateAttribute(XML_Constants.OPERATION);
            operationAttr.Value = Enum<SearchOperationType>.EnumText(this.OperationType);
            searchOpElement.Attributes.Append(operationAttr);

            return searchOpElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Role' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationRole : PresentationXmlElement, IGOSToXml
    {
        private RoleType roleType;

        /// <summary>
        /// The roleId attribute value - returns a RoleType enum value
        /// </summary>
        public RoleType RoleType
        {
            get { return roleType; }
            set { roleType = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement roleElement = xmlDoc.CreateElement(XML_Constants.ROLE);

            XmlAttribute roleIdAttr = xmlDoc.CreateAttribute(XML_Constants.ROLE_ID);
            roleIdAttr.Value = Enum<RoleType>.EnumText(RoleType);
            roleElement.Attributes.Append(roleIdAttr);

            return roleElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Enum' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationEnumeration : DisplayItem, IGOSToXml
    {
        private string searchValue;
        private string origImagePath, imagePath;

        /// <summary>
        /// The inner text value of the 'Enum' element - use to create search expression set xml
        /// </summary>
        public string SearchValue 
        {
            get { return searchValue; }
            set { searchValue = value; } 
        }

        /// <summary>
        /// The 'imagePath' attribute value - this value was appended to the rootImagePath attribute value
        /// </summary>
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        internal string OriginalImagePath
        {
            set { origImagePath = value; }
            get { return origImagePath; }
        }

        #region IGOSToXml Members

        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement enumElement = xmlDoc.CreateElement(XML_Constants.ENUM);

            XmlAttribute resourceIDAttr = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
            resourceIDAttr.Value = this.ResourceID;
            enumElement.Attributes.Append(resourceIDAttr);

            XmlAttribute displayNameAttr = xmlDoc.CreateAttribute(XML_Constants.DISPLAY_NAME);
            displayNameAttr.Value = this.DisplayName;
            enumElement.Attributes.Append(displayNameAttr);

            XmlAttribute imgPathAttr = xmlDoc.CreateAttribute(XML_Constants.IMAGE_PATH);
            //imgPathAttr.Value = this.ImagePath;
            imgPathAttr.Value = this.OriginalImagePath;
            enumElement.Attributes.Append(imgPathAttr);

            enumElement.InnerText = this.SearchValue;

            return enumElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Object' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationObject : DisplayItem, IGOSToXml
    {
        private string id;
        private string refId;
        private string origImagePath, imagePath;
        private List<PresentationCriterionView> criteriaViews;
        private List<PresentationResultView> resultViews;
        private List<PresentationPreviewView> previewViews;
        private List<PresentationPropertyAlias> propertyAliases;
        private List<PresentationRole> searchRoles;
        private GroupingItem associatedGroup;

        /// <summary>
        /// Constructs an empty presentation object
        /// </summary>
        public PresentationObject()
        {
            this.criteriaViews = new List<PresentationCriterionView>();
            this.resultViews = new List<PresentationResultView>();
            this.previewViews = new List<PresentationPreviewView>();
            this.propertyAliases = new List<PresentationPropertyAlias>();
            this.searchRoles = new List<PresentationRole>();
        }

        /// <summary>
        /// Constructs a default presentation object based on the npm object
        /// </summary>
        /// <param name="npmObject"></param>
        public PresentationObject(XmlNode npmObject, List<PresentationProperty> objProps) : this()
        {
            this.ID = AttributeRetrievalHelper.GetStringValue(npmObject.Attributes, XML_Constants.NPMID);
            this.DisplayName = AttributeRetrievalHelper.GetStringValue(npmObject.Attributes, XML_Constants.NAME);
            this.ResourceID = AttributeRetrievalHelper.GetResourceIdFromName(this.DisplayName, ResourceIdType.Object);

            // add the default criteria views
            this.AddDefaultCriteriaViews(objProps);

            // add the default result views
            this.AddDefaultResultViews(objProps);

            // add the default preview views
            this.AddDefaultPreviewViews(objProps);
        }

        /// <summary>
        /// Copy CTOR
        /// </summary>
        /// <param name="PresObject"></param>
        internal PresentationObject(PresentationObject PresObject)
        {
            criteriaViews = Cloner.DeepClone(PresObject.CriteriaViews) as List<PresentationCriterionView>;

            resultViews = Cloner.DeepClone(PresObject.ResultViews) as List<PresentationResultView>;

            previewViews = Cloner.DeepClone(PresObject.PreviewViews) as List<PresentationPreviewView>;

            propertyAliases = Cloner.DeepClone(PresObject.PropertyAliases) as List<PresentationPropertyAlias>;

            searchRoles = Cloner.DeepClone(PresObject.Roles) as List<PresentationRole>;
        }

        /// <summary>
        /// The 'id' attribute value - ties directly to an NPM object in the NPM definition
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string RefId
        {
            get { return this.refId; }
            set { this.refId = value; }
        }

        /// <summary>
        /// The 'imagePath' attribute value - this value was appended to the rootImagePath 
        /// </summary>
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        internal string OriginalImagePath
        {
            set { origImagePath = value; }
            get { return origImagePath; }
        }

        /// <summary>
        /// Gets a list of PresentationCriterionView objects
        /// </summary>
        public List<PresentationCriterionView> CriteriaViews 
        {
            get { return criteriaViews; }
            internal set { criteriaViews = value; }
        }

        /// <summary>
        /// Gets a list of PresentationRole objects associated with this object instance
        /// </summary>
        public List<PresentationRole> Roles
        {
            get { return searchRoles; }
            internal set { searchRoles = value; }
        }

        /// <summary>
        /// Gets a list of PresentationResultView objects
        /// </summary>
        public List<PresentationResultView> ResultViews 
        {
            get { return resultViews; }
            internal set { resultViews = value; }
        }

        /// <summary>
        /// Gets a list of PresentationPreviewView objects
        /// </summary>
        public List<PresentationPreviewView> PreviewViews 
        {
            get { return previewViews; }
            internal set { previewViews = value; }
        }

        public List<PresentationPropertyAlias> PropertyAliases
        {
            get { return propertyAliases; }
            internal set { propertyAliases = value; }
        }

        /// <summary>
        /// Gets the GroupingItem in which this object instance is associated
        /// </summary>
        public GroupingItem AssociatedGroup
        {
            get { return associatedGroup; }
            internal set { associatedGroup = value; }
        }

        private void AddDefaultCriteriaViews(List<PresentationProperty> objProps)
        {
            PresentationCriterionView cView = new PresentationCriterionView(objProps);

            this.CriteriaViews.Add(cView);
        }

        private void AddDefaultResultViews(List<PresentationProperty> objProps)
        {
            PresentationResultView rView = new PresentationResultView(objProps);

            rView.ID = 1;
            rView.DisplayName = this.DisplayName + " " + rView.ID;
            rView.ResourceID = AttributeRetrievalHelper.GetResourceIdFromName(rView.DisplayName, ResourceIdType.Result_View);

            this.ResultViews.Add(rView);
        }

        private void AddDefaultPreviewViews(List<PresentationProperty> objProps)
        {
            PresentationPreviewView pView = new PresentationPreviewView(objProps);

            this.PreviewViews.Add(pView);
        }

        /// <summary>
        /// Finds a PresentationCriterionView instance by ID
        /// </summary>
        /// <param name="ID">ID of the PresentationCriterionView to retrieve</param>
        /// <returns>PresentationCriterionView instance</returns>
        public PresentationCriterionView GetCriterionView(int ID)
        {
            return criteriaViews.Find(delegate(PresentationCriterionView criterionView)
            { return criterionView.ID == ID; });
        }

        public PresentationPropertyAlias GetPropertyAlias(string PresentationPropId)
        {
            if (!string.IsNullOrEmpty(PresentationPropId))
            {
                return propertyAliases.Find(delegate(PresentationPropertyAlias propAlias)
                    { return propAlias.AssociatedPresentationOnlyProperty.PresentationPropId == PresentationPropId; });
            }
            return null;
        }

        /// <summary>
        /// Retrieves a PresentationRole based upon role type
        /// </summary>
        /// <param name="RoleType"></param>
        /// <returns></returns>
        public PresentationRole GetSearchRole(RoleType RoleType)
        {
            return Roles.Find(delegate(PresentationRole role)
            { return role.RoleType == RoleType; });
        }

        /// <summary>
        /// Finds a PresentationPreviewView instance by ID
        /// </summary>
        /// <param name="ID">ID of the PresentationPreviewView to retrieve</param>
        /// <returns>PresentationPreviewView instance</returns>
        public PresentationPreviewView GetPreviewView(int ID)
        {
            return previewViews.Find(delegate(PresentationPreviewView previewView)
            { return previewView.ID == ID; });
        }

        /// <summary>
        /// Finds a PresentationResultView instance by ID
        /// </summary>
        /// <param name="ID">ID of the PresentationResultView to retrieve</param>
        /// <returns>PresentationResultView instance</returns>
        public PresentationResultView GetResultView(int ID)
        {
            return resultViews.Find(delegate(PresentationResultView resultView)
            { return resultView.ID == ID; });
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement objElement = xmlDoc.CreateElement(XML_Constants.OBJECT);

            if (string.IsNullOrEmpty(this.RefId) == false)
            {
                XmlAttribute refIdAttr = xmlDoc.CreateAttribute(XML_Constants.REF_ID);
                refIdAttr.Value = this.RefId;
                objElement.Attributes.Append(refIdAttr);
            }

            XmlAttribute idAttr = xmlDoc.CreateAttribute(XML_Constants.ID);
            idAttr.Value = this.ID;
            objElement.Attributes.Append(idAttr);

            XmlAttribute resourceIDAttr = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
            resourceIDAttr.Value = this.ResourceID;
            objElement.Attributes.Append(resourceIDAttr);

            XmlAttribute displayNameAttr = xmlDoc.CreateAttribute(XML_Constants.DISPLAY_NAME);
            displayNameAttr.Value = this.DisplayName;
            objElement.Attributes.Append(displayNameAttr);

            XmlAttribute imgPathAttr = xmlDoc.CreateAttribute(XML_Constants.IMAGE_PATH);
            //imgPathAttr.Value = this.ImagePath;
            imgPathAttr.Value = this.OriginalImagePath;
            objElement.Attributes.Append(imgPathAttr);

            if (string.IsNullOrEmpty(this.RefId) == true)
            {
                XmlElement viewsElement = xmlDoc.CreateElement(XML_Constants.VIEWS);

                if (this.PropertyAliases != null && this.PropertyAliases.Count > 0)
                {
                    XmlElement paViewElement = xmlDoc.CreateElement(XML_Constants.PRES_PROP_ALIASES);

                    foreach (IGOSToXml cView in this.PropertyAliases)
                    {
                        paViewElement.AppendChild(cView.ToXml(xmlDoc));
                    }

                    objElement.AppendChild(paViewElement);
                }

                if (this.CriteriaViews != null && this.CriteriaViews.Count > 0)
                {
                    XmlElement cViewElement = xmlDoc.CreateElement(XML_Constants.CRITERION_VIEWS);

                    foreach (IGOSToXml cView in this.CriteriaViews)
                    {
                        cViewElement.AppendChild(cView.ToXml(xmlDoc));
                    }

                    viewsElement.AppendChild(cViewElement);
                }

                if (this.ResultViews != null && this.ResultViews.Count > 0)
                {
                    XmlElement rViewElement = xmlDoc.CreateElement(XML_Constants.RESULT_VIEWS);

                    foreach (IGOSToXml rView in this.ResultViews)
                    {
                        rViewElement.AppendChild(rView.ToXml(xmlDoc));
                    }

                    viewsElement.AppendChild(rViewElement);
                }

                if (this.PreviewViews != null && this.PreviewViews.Count > 0)
                {
                    XmlElement pViewElement = xmlDoc.CreateElement(XML_Constants.PREVIEW_VIEWS);

                    foreach (IGOSToXml pView in this.PreviewViews)
                    {
                        pViewElement.AppendChild(pView.ToXml(xmlDoc));
                    }

                    viewsElement.AppendChild(pViewElement);
                }

                objElement.AppendChild(viewsElement);
            }

            return objElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'PreviewView' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationPreviewView : PresentationXmlElement, IGOSToXml
    {
        private int id;
        private string xsltLocation;
        private List<PresentationPreviewItem> previewItems;

        public PresentationPreviewView()
        {
            this.previewItems = new List<PresentationPreviewItem>();
        }

        public PresentationPreviewView(List<PresentationProperty> objProps) : this()
        {
            this.AddDefaultPreviewItems(objProps);
        }

        /// <summary>
        /// The 'id' attribute value
        /// </summary>
        public int ID 
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The 'xsltLocation' attribute value
        /// </summary>
        public string XsltLocation 
        {
            get { return xsltLocation; }
            set { xsltLocation = value; }
        }

        /// <summary>
        /// Gets a list of PresentationPreviewItem objects
        /// </summary>
        public List<PresentationPreviewItem> PreviewItems 
        {
            get { return previewItems; }
            internal set { previewItems = value; }
        }

        private void AddDefaultPreviewItems(List<PresentationProperty> objProps)
        {
            const int KeywordPropId = 10;
            int previewIndex = 0;

            foreach (PresentationProperty prop in objProps)
            {
                if (prop.IsPresentationOnlyProperty == false && prop.NPMPropID != KeywordPropId)
                {
                    PresentationPreviewItem previewItem = new PresentationPreviewItem();
                    previewItem.AssociatedProperty = prop;
                    previewItem.NPMPropID = prop.NPMPropID;
                    previewItem.NPMObjectID = prop.NPMObjectID;
                    previewItem.PreviewIndex = previewIndex++;

                    this.PreviewItems.Add(previewItem);
                }
            }
        }

        /// <summary>
        /// Retrieves a PresentationPreviewItem instance by NpmPropID
        /// </summary>
        /// <param name="NpmPropID">ID of PresentationPreviewItem to retrieve </param>
        /// <returns>PresentationPreviewItem instance</returns>
        public PresentationPreviewItem GetPreviewItem(int NpmPropID)
        {
            return previewItems.Find(delegate(PresentationPreviewItem previewItem)
                { return previewItem.NPMPropID == NpmPropID; });
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement pViewElement = xmlDoc.CreateElement(XML_Constants.PREVIEW_VIEW);

            XmlAttribute idAttr = xmlDoc.CreateAttribute(XML_Constants.ID);
            idAttr.Value = this.ID.ToString();
            pViewElement.Attributes.Append(idAttr);

            XmlAttribute xsltAttr = xmlDoc.CreateAttribute(XML_Constants.XSLT_LOCATION);
            xsltAttr.Value = this.XsltLocation;
            pViewElement.Attributes.Append(xsltAttr);

            PreviewItems.Sort();
            if (this.PreviewItems != null && this.PreviewItems.Count > 0)
            {
                foreach (IGOSToXml pViewItem in this.PreviewItems)
                {
                    pViewElement.AppendChild(pViewItem.ToXml(xmlDoc));
                }
            }

            return pViewElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'PreviewItem' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationPreviewItem : PresentationPropertyDerived, IGOSToXml, IComparable
    {
        private int npmObjectID = -1;
        private int npmPropID;
        private int previewIndex;

        /// <summary>
        /// The 'npmObjectId' attribute value
        /// </summary>
        public int NPMObjectID 
        {
            get { return npmObjectID; }
            set { npmObjectID = value; }
        }

        /// <summary>
        /// The 'npmPropId' attribute value
        /// </summary>
        public int NPMPropID 
        {
            get { return npmPropID; }
            set { npmPropID = value; }
        }

        /// <summary>
        /// The 'previewIndex' attribute value
        /// </summary>
        public int PreviewIndex 
        {
            get { return previewIndex; }
            set { previewIndex = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serialize this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement pViewItemElement = xmlDoc.CreateElement(XML_Constants.PREVIEW_ITEM);

            if (this.NPMObjectID >= 0)
            {
                XmlAttribute npmObjIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_OBJECT_ID);
                npmObjIdAttr.Value = this.NPMObjectID.ToString();
                pViewItemElement.Attributes.Append(npmObjIdAttr);
            }

            XmlAttribute npmPropIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_PROP_ID);
            npmPropIdAttr.Value = this.NPMPropID.ToString();
            pViewItemElement.Attributes.Append(npmPropIdAttr);

            XmlAttribute pIndexAttr = xmlDoc.CreateAttribute(XML_Constants.PREVIEW_INDEX);
            pIndexAttr.Value = this.PreviewIndex.ToString();
            pViewItemElement.Attributes.Append(pIndexAttr);

            if (!string.IsNullOrEmpty(base.resourceId))
            {
                XmlAttribute resourceId = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
                resourceId.Value = base.resourceId;
                pViewItemElement.Attributes.Append(resourceId);
            }

            return pViewItemElement;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Sorts this instance by the PreviewIndex value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is PresentationPreviewItem)
            {
                PresentationPreviewItem otherPresentationPreviewItem = (PresentationPreviewItem)obj;
                return this.PreviewIndex.CompareTo(otherPresentationPreviewItem.PreviewIndex);
            }
            else
            {
                throw new ArgumentException("Object is not of type PresentationPreviewItem");
            }
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'CriterionView' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationCriterionView : PresentationXmlElement, IGOSToXml
    {
        private int id;
        private List<PresentationCriteria> criterionList;

        public PresentationCriterionView()
        {
            this.criterionList = new List<PresentationCriteria>();
        }

        /// <summary>
        /// Constructs a default criteria view based on the list of object properties
        /// </summary>
        /// <param name="objProps"></param>
        public PresentationCriterionView(List<PresentationProperty> objProps) : this()
        {
            this.AddDefaultCriteria(objProps);
        }

        /// <summary>
        /// The 'id' attribute value
        /// </summary>
        public int ID 
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets a list of PresentationCriteria objects
        /// </summary>
        public List<PresentationCriteria> CriterionList 
        {
            get { return criterionList; }
            internal set { criterionList = value; }
        }

        private void AddDefaultCriteria(List<PresentationProperty> objProps)
        {
            int criteriaIndex = 0;

            foreach (PresentationProperty prop in objProps)
            {
                if (prop.IsSearchable == true)
                {
                    // create the criteria based on the property xml blob
                    PresentationCriteria criteria = new PresentationCriteria();
                    criteria.IsDefault = (criteriaIndex < 3);
                    criteria.CriteriaIndex = criteriaIndex++;
                    criteria.NPMPropID = prop.NPMPropID;
                    criteria.AssociatedProperty = prop;

                    // if the property is not a common property, then parse the object ID
                    if (prop.NPMObjectID > -1)
                    {
                        criteria.NPMObjectID = prop.NPMObjectID;
                    }

                    this.CriterionList.Add(criteria);
                }
            }
        }

        /// <summary>
        /// Finds a PresentationCriteria object by NPMPropID
        /// </summary>
        /// <param name="NpmPropId">NPMPropID of the PresentationCriteria instance to find</param>
        /// <returns>PresentationCriteria instance</returns>
        public PresentationCriteria GetPresentationCriteriaInstance(int NpmPropId)
        {
            return criterionList.Find(delegate(PresentationCriteria criteria)
            { return criteria.NPMPropID == NpmPropId; });
        }

        /// <summary>
        /// Determines if a PresentationCriteria object exists in the criterion list
        /// </summary>
        /// <param name="c">PresentationCriteria instance</param>
        /// <returns>True if exists, False if not</returns>
        public bool ContainsCriteria(PresentationCriteria c)
        {
            bool returnValue = false;

            foreach (PresentationCriteria criteria in this.CriterionList)
            {
                if (criteria.NPMPropID == c.NPMPropID)
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        public List<PresentationCriteria> GetDefaultCriteria()
        {
            return criterionList.FindAll(delegate(PresentationCriteria criteria) { return criteria.IsDefault == true; });
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement cViewElement = xmlDoc.CreateElement(XML_Constants.CRITERION_VIEW);

            XmlAttribute idAttr = xmlDoc.CreateAttribute(XML_Constants.ID);
            idAttr.Value = this.ID.ToString();
            cViewElement.Attributes.Append(idAttr);

            if (this.CriterionList != null && this.CriterionList.Count > 0)
            {
                this.CriterionList.Sort();
                foreach (IGOSToXml cViewItem in this.CriterionList)
                {
                    cViewElement.AppendChild(cViewItem.ToXml(xmlDoc));
                }
            }

            return cViewElement;
        }

        #endregion
    }

    [Serializable]
    public class PresentationPropertyAlias : PresentationXmlElement, IGOSToXml
    {

        private PresentationProperty associateNpmProperty;
        private PresentationProperty associatePresOnlyProperty;

        int npmPropId = -1, npmObjectId = -1;

        string presPropId;

        /// <summary>
        /// The npmObjectId attribute value 
        /// </summary>
        public int NPMObjectID
        {
            get { return npmObjectId; }
            set { npmObjectId = value; }
        }

        /// <summary>
        /// The npmPropId attribute value
        /// </summary>
        public int NPMPropID
        {
            get { return npmPropId; }
            set { npmPropId = value; }
        }

        public string PresentationOnlyPropID
        {
            get { return presPropId; }
            set { presPropId = value; }
        }

        /// <summary>
        /// The associated NPM type property 
        /// </summary>
        public PresentationProperty AssociatedNPMProperty
        {
            get { return associateNpmProperty; }
            set { associateNpmProperty = value; }
        }

        /// <summary>
        /// The associated presentation only property
        /// </summary>
        public PresentationProperty AssociatedPresentationOnlyProperty
        {
            get { return associatePresOnlyProperty; }
            set { associatePresOnlyProperty = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement criteriaElement = xmlDoc.CreateElement(XML_Constants.PRES_PROP_ALIAS);

            if (this.NPMObjectID >= 0)
            {
                XmlAttribute npmObjIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_OBJECT_ID);
                npmObjIdAttr.Value = this.NPMObjectID.ToString();
                criteriaElement.Attributes.Append(npmObjIdAttr);
            }

            if (this.NPMPropID >= 0)
            {
                XmlAttribute npmPropIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_PROP_ID);
                npmPropIdAttr.Value = this.NPMPropID.ToString();
                criteriaElement.Attributes.Append(npmPropIdAttr);
            }

            if (!string.IsNullOrEmpty(this.presPropId))
            {
                XmlAttribute presPropIdAttr = xmlDoc.CreateAttribute(XML_Constants.PRES_PROP_ID);
                presPropIdAttr.Value = this.presPropId;
                criteriaElement.Attributes.Append(presPropIdAttr);
            }

            return criteriaElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Criteria' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationCriteria : PresentationPropertyDerived, IComparable, IGOSToXml
    {
        private int npmObjectId = -1;
        private int npmPropId;
        private int criteriaIndex;
        private bool isDefault;
        private string presPropId;

        /// <summary>
        /// The npmObjectId attribute value 
        /// </summary>
        public int NPMObjectID
        {
            get { return npmObjectId; }
            set { npmObjectId = value; }
        }

        /// <summary>
        /// The npmPropId attribute value
        /// </summary>
        public int NPMPropID 
        {
            get { return npmPropId; }
            set { npmPropId = value; }
        }

        /// <summary>
        /// The 'presPropId' attribute value
        /// </summary>
        public string PresPropID
        {
            get { return this.presPropId; }
            set { this.presPropId = value; }
        }

        /// <summary>
        /// The 'criteriaIndex' attribute value
        /// </summary>
        public int CriteriaIndex 
        {
            get { return criteriaIndex; }
            set { criteriaIndex = value; }
        }

        /// <summary>
        /// The 'isDefault' attribute value
        /// </summary>
        public bool IsDefault 
        {
            get { return isDefault; }
            set { isDefault = value; }
        }

        #region IComparable Members
        /// <summary>
        /// Provides sorting logic for lists of this instance - sorted by criteria index
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is PresentationCriteria)
            {
                PresentationCriteria otherCriteria = (PresentationCriteria)obj;
                return this.criteriaIndex.CompareTo(otherCriteria.CriteriaIndex);
            }
            else
            {
                throw new ArgumentException("Object is not of type PresentationCriteria");
            }
        }

        #endregion

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement criteriaElement = xmlDoc.CreateElement(XML_Constants.CRITERIA);

            if (string.IsNullOrEmpty(this.PresPropID) == true)
            {
                if (this.NPMObjectID >= 0)
                {
                    XmlAttribute npmObjIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_OBJECT_ID);
                    npmObjIdAttr.Value = this.NPMObjectID.ToString();
                    criteriaElement.Attributes.Append(npmObjIdAttr);
                }

                XmlAttribute npmPropIdAttr = xmlDoc.CreateAttribute(XML_Constants.NPM_PROP_ID);
                npmPropIdAttr.Value = this.NPMPropID.ToString();
                criteriaElement.Attributes.Append(npmPropIdAttr);
            }
            else
            {
                XmlAttribute presPropIdAttr = xmlDoc.CreateAttribute(XML_Constants.PRES_PROP_ID);
                presPropIdAttr.Value = this.PresPropID;
                criteriaElement.Attributes.Append(presPropIdAttr);
            }

            XmlAttribute cIndexAttr = xmlDoc.CreateAttribute(XML_Constants.CRITERIA_INDEX);
            cIndexAttr.Value = this.CriteriaIndex.ToString();
            criteriaElement.Attributes.Append(cIndexAttr);

            XmlAttribute isDefaultAttr = xmlDoc.CreateAttribute(XML_Constants.IS_DEFAULT);
            isDefaultAttr.Value = this.IsDefault.ToString().ToLower();
            criteriaElement.Attributes.Append(isDefaultAttr);

            if (!string.IsNullOrEmpty(base.resourceId))
            {
                XmlAttribute resourceId = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
                resourceId.Value = base.resourceId;
                criteriaElement.Attributes.Append(resourceId);
            }

            return criteriaElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'ResultView' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationResultView : DisplayItem, IGOSToXml
    {
        private int id;
        private List<PresentationColumn> columnList;

        public PresentationResultView()
        {
            this.columnList = new List<PresentationColumn>();
        }

        /// <summary>
        /// Constructs a default result view based on the object properties
        /// </summary>
        /// <param name="objProps"></param>
        public PresentationResultView(List<PresentationProperty> objProps) : this()
        {
            this.AddDefaultColumns(objProps);
        }

        /// <summary>
        /// The 'id' attribute value
        /// </summary>
        public int ID 
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets a list of encapsulated PresentationColumn objects
        /// </summary>
        public List<PresentationColumn> ColumnList
        {
            get { return columnList; }
            internal set { columnList = value; }
        }

        private void AddDefaultColumns(List<PresentationProperty> objProps)
        {
            int resultIndex = 0;

            foreach (PresentationProperty prop in objProps)
            {
                if (prop.IsSearchHit == true)
                {
                    int index = resultIndex++;

                    // create the presentation column based on the object property
                    PresentationColumn column = new PresentationColumn();
                    column.NPMPropID = prop.NPMPropID;
                    column.ResultIndex = index;
                    column.ResultSortDirection = SortDirection.Asc;
                    column.GroupByIndex = index;
                    column.GroupBySortDirection = SortDirection.Asc;
                    column.GroupBySortPriority = index;
                    column.IsGroupBy = false;
                    column.IsGroupBySorted = false;
                    column.IsSortable = true;
                    column.IsSorted = false;
                    column.IsVisible = true;
                    column.SortPriority = index;
                    column.AssociatedProperty = prop;
                    column.IsCaptionVisible = true;
                    column.IsCaptionImgVisible = false;

                    // if the property is not a common property, then parse the object ID
                    if (prop.NPMObjectID > -1)
                    {
                        column.NPMObjectID = prop.NPMObjectID;
                    }

                    this.ColumnList.Add(column);
                }
            }
        }
       
        /// <summary>
        /// Retrieves a PresentationColumn object instance by NPM Prop ID
        /// </summary>
        /// <param name="NpmPropId">The NPM Prop ID of the PresentationColumn instance to retrieve</param>
        /// <returns>PresentationColumn instance</returns>
        public PresentationColumn GetPresentationColumnInstance(int NpmPropId)
        {
            return columnList.Find(delegate(PresentationColumn column)
            { return column.NPMPropID == NpmPropId; });
        }

        
        public PresentationColumn GetPresentationColumnInstanceByPresPropID(string PresentationPropID)
        {
            return columnList.Find(delegate(PresentationColumn column)
            { return column.AssociatedProperty.PresentationPropId == PresentationPropID; });
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement rViewElement = xmlDoc.CreateElement(XML_Constants.RESULT_VIEW);

            XmlAttribute idAttr = xmlDoc.CreateAttribute(XML_Constants.ID);
            idAttr.Value = this.ID.ToString();
            rViewElement.Attributes.Append(idAttr);

            XmlAttribute resourceIDAttr = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
            resourceIDAttr.Value = this.ResourceID;
            rViewElement.Attributes.Append(resourceIDAttr);

            XmlAttribute displayNameAttr = xmlDoc.CreateAttribute(XML_Constants.DISPLAY_NAME);
            displayNameAttr.Value = this.DisplayName;
            rViewElement.Attributes.Append(displayNameAttr);

            if (this.ColumnList != null && this.ColumnList.Count > 0)
            {
                XmlElement columnsElement = xmlDoc.CreateElement(XML_Constants.COLUMNS);

                this.ColumnList.Sort();
                foreach (IGOSToXml col in this.ColumnList)
                {
                    columnsElement.AppendChild(col.ToXml(xmlDoc));
                }

                rViewElement.AppendChild(columnsElement);
            }

            return rViewElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Column' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationColumn : PresentationPropertyDerived, IComparable, IGOSToXml
    { 
        private int npmObjectId = -1;
        private int npmPropId;
        private int groupByIndex;
        private int groupBySortPriority;
        private int sortPriority;
        private int resultIndex;

        private bool isGroupBy;
        private bool isGroupBySorted;
        private bool isSortable;
        private bool isSorted;
        private bool isVisible;
        private bool isCaptionImgVisible;
        private bool isCaptionVisible = true;

        private string presPropId;

        private SortDirection groupBySortDirection;
        private SortDirection resultSortDirection;

        /// <summary>
        /// The 'npmObjectId' attribute value
        /// </summary>
        public int NPMObjectID 
        {
            get { return npmObjectId; }
            set { npmObjectId = value; }
        }

        /// <summary>
        /// The 'npmPropId' attribute value
        /// </summary>
        public int NPMPropID 
        {
            get { return npmPropId; }
            set { npmPropId = value; }
        }

        /// <summary>
        /// The 'presPropId' attribute value
        /// </summary>
        public string PresPropID
        {
            get { return this.presPropId; }
            set { this.presPropId = value; }
        }

        /// <summary>
        /// The 'groupByIndex' attribute value
        /// </summary>
        public int GroupByIndex 
        {
            get { return groupByIndex; }
            set { groupByIndex = value; }
        }

        /// <summary>
        /// The 'groupBySortPriority' attribute value
        /// </summary>
        public int GroupBySortPriority
        {
            get { return groupBySortPriority; }
            set { groupBySortPriority = value; }
        }

        /// <summary>
        /// The 'sortPriority' attribute value
        /// </summary>
        public int SortPriority 
        {
            get { return sortPriority; }
            set { sortPriority = value; }
        }

        /// <summary>
        /// The 'resultIndex' attribute value
        /// </summary>
        public int ResultIndex 
        {
            get { return resultIndex; }
            set { resultIndex = value; }
        }

        /// <summary>
        /// The 'isGroupBy' attribute value
        /// </summary>
        public bool IsGroupBy 
        {
            get { return isGroupBy; }
            set { isGroupBy = value; }
        }

        /// <summary>
        /// The 'isCaptionVisible' attribute value
        /// </summary>
        public bool IsCaptionVisible
        {
            get { return isCaptionVisible; }
            set { isCaptionVisible = value; }
        }

        /// <summary>
        /// The 'isCaptionImgVisible' attribute value
        /// </summary>
        public bool IsCaptionImgVisible
        {
            get { return isCaptionImgVisible; }
            set { isCaptionImgVisible = value; }
        }

        /// <summary>
        /// The 'isGroupBySorted' attribute value
        /// </summary>
        public bool IsGroupBySorted 
        {
            get { return isGroupBySorted; }
            set { isGroupBySorted = value; }
        }

        /// <summary>
        /// The 'isSortable' attribute value
        /// </summary>
        public bool IsSortable 
        {
            get { return isSortable; }
            set { isSortable = value; }
        }

        /// <summary>
        /// The 'isSorted' attribute value
        /// </summary>
        public bool IsSorted 
        {
            get { return isSorted; }
            set { isSorted = value; }
        }

        /// <summary>
        /// The 'isVisible' attribute value
        /// </summary>
        public bool IsVisible 
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// The 'groupBySortDir' attribute value - returns SortDirection enum Desc or Asc
        /// </summary>
        public SortDirection GroupBySortDirection 
        {
            get { return groupBySortDirection; }
            set { groupBySortDirection = value; }
        }

        /// <summary>
        /// The 'resultSortDir' attribute value - returns SortDirection enum Desc or Asc
        /// </summary>
        public SortDirection ResultSortDirection 
        {
            get { return resultSortDirection; }
            set { resultSortDirection = value; }
        }
        
        #region IComparable Members

        /// <summary>
        /// Sorting logic implementation - columns are sorted by result index
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is PresentationColumn)
            {
                PresentationColumn otherColumn = (PresentationColumn)obj;
                return this.resultIndex.CompareTo(otherColumn.ResultIndex);
            }
            else
            {
                throw new ArgumentException("Object is not of type PresentationColumn");
            }
        }

        #endregion

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement colElement = xmlDoc.CreateElement(XML_Constants.COLUMN);
            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(this.PresPropID) == true)
            {
                if (this.NPMObjectID >= 0)
                {
                    attr = xmlDoc.CreateAttribute(XML_Constants.NPM_OBJECT_ID);
                    attr.Value = this.NPMObjectID.ToString();
                    colElement.Attributes.Append(attr);
                }

                attr = xmlDoc.CreateAttribute(XML_Constants.NPM_PROP_ID);
                attr.Value = this.NPMPropID.ToString();
                colElement.Attributes.Append(attr);
            }
            else
            {
                attr = xmlDoc.CreateAttribute(XML_Constants.PRES_PROP_ID);
                attr.Value = this.PresPropID;
                colElement.Attributes.Append(attr);
            }

            attr = xmlDoc.CreateAttribute(XML_Constants.RESULT_INDEX);
            attr.Value = this.ResultIndex.ToString();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_VISIBLE);
            attr.Value = this.IsVisible.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_CAPTION_IMG_VISIBLE);
            attr.Value = this.IsCaptionImgVisible.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_CAPTION_VISIBLE);
            attr.Value = this.IsCaptionVisible.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_SORTABLE);
            attr.Value = this.IsSortable.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_SORTED);
            attr.Value = this.IsSorted.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.RESULT_SORT_DIR);
            attr.Value = Enum<SortDirection>.EnumText(this.ResultSortDirection);
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.SORT_PRIORITY);
            attr.Value = this.SortPriority.ToString();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_GROUP_BY);
            attr.Value = this.IsGroupBy.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.GROUP_BY_INDEX);
            attr.Value = this.GroupByIndex.ToString();
            colElement.Attributes.Append(attr); 

            attr = xmlDoc.CreateAttribute(XML_Constants.IS_GROUP_BY_SORTED);
            attr.Value = this.IsGroupBySorted.ToString().ToLower();
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.GROUP_BY_SORT_DIR);
            attr.Value = Enum<SortDirection>.EnumText(this.GroupBySortDirection);
            colElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(XML_Constants.GROUP_BY_SORT_PRIORITY);
            attr.Value = this.GroupBySortPriority.ToString();
            colElement.Attributes.Append(attr);

            if (!string.IsNullOrEmpty(base.resourceId))
            {
                XmlAttribute resourceId = xmlDoc.CreateAttribute(XML_Constants.RESOURCE_ID);
                resourceId.Value = base.resourceId;
                colElement.Attributes.Append(resourceId);
            }

            return colElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Group' element in the Presentation XML
    /// </summary>
    [Serializable]
    public class PresentationGroup : GroupingItem , IGOSToXml
    {
        private string objectId;
        private bool isVisible = true;
        private List<PresentationGroup> groups;
        private List<PresentationGroupItem> groupItems;

        public PresentationGroup()
        {
            this.groups = new List<PresentationGroup>();
            this.groupItems = new List<PresentationGroupItem>();
        }

        /// <summary>
        /// The 'objectId' attribute value
        /// </summary>
        public string ObjectID 
        {
            get { return objectId; }
            set { objectId = value; }
        }

        /// <summary>
        /// Determines whether or not to show this group in the UI
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// Gets a list of encapsulated PresentationGroup objects
        /// </summary>
        public List<PresentationGroup> Groups 
        {
            get { return groups; }
            internal set { groups = value; }
        }

        /// <summary>
        /// Gets a list of encapsulated PresentationGroupItem objects
        /// </summary>
        public List<PresentationGroupItem> GroupItems 
        {
            get { return groupItems; }
            internal set { groupItems = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement grpElement = xmlDoc.CreateElement(XML_Constants.GROUP);

            XmlAttribute objIdAttr = xmlDoc.CreateAttribute(XML_Constants.OBJECT_ID);
            objIdAttr.Value = this.ObjectID;
            grpElement.Attributes.Append(objIdAttr);

            XmlAttribute isVisible = xmlDoc.CreateAttribute(XML_Constants.IS_VISIBLE);
            isVisible.Value = this.isVisible.ToString().ToLower();
            grpElement.Attributes.Append(isVisible);

            if (this.Groups != null && this.Groups.Count > 0)
            {
                foreach (IGOSToXml grp in this.Groups)
                {
                    grpElement.AppendChild(grp.ToXml(xmlDoc));
                }
            }

            if (this.GroupItems != null && this.GroupItems.Count > 0)
            {
                foreach (IGOSToXml grpItem in this.GroupItems)
                {
                    grpElement.AppendChild(grpItem.ToXml(xmlDoc));                    
                }
            }

            return grpElement;
        }

        #endregion
    }

    /// <summary>
    /// Class to encapsulate the 'Item' element in the Presentation XML 
    /// </summary>
    [Serializable]
    public class PresentationGroupItem : GroupingItem, IGOSToXml
    {
        private string objectId;
        private bool isVisible = true;

        /// <summary>
        /// The 'objectId' attribute value
        /// </summary>
        public string ObjectID
        {
            get { return objectId; }
            set { objectId = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        #region IGOSToXml Members
        /// <summary>
        /// Serializes this instance to XML
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlElement ToXml(XmlDocument xmlDoc)
        {
            XmlElement grpItemElement = xmlDoc.CreateElement(XML_Constants.ITEM);

            XmlAttribute objIdAttr = xmlDoc.CreateAttribute(XML_Constants.OBJECT_ID);
            objIdAttr.Value = this.ObjectID;
            grpItemElement.Attributes.Append(objIdAttr);

            XmlAttribute isVisible = xmlDoc.CreateAttribute(XML_Constants.IS_VISIBLE);
            isVisible.Value = this.isVisible.ToString().ToLower();
            grpItemElement.Attributes.Append(isVisible);

            return grpItemElement;
        }

        #endregion
    }
    #endregion

    #region Enums

    public enum ResourceIdType
    {
        Prop,
        Enum,
        Object,
        Result_View
    }

    public enum SortDirection
    {
        [Description(XML_Constants.ASC)]
        Asc,
        [Description(XML_Constants.DESC)]
        Desc
    }

    public enum FieldDispType
    {
        [Description(XML_Constants.FIELD_DISPLAY_TEXT)]
        Text,
        [Description(XML_Constants.FIELD_DISPLAY_BOOLEAN)]
        Boolean,
        [Description(XML_Constants.FIELD_DISPLAY_DATETIME)]
        DateTime,
        [Description(XML_Constants.FIELD_DISPLAY_ADDRESS)]
        Address,
        [Description(XML_Constants.FIELD_DISPLAY_NUMBER)]
        Number,
        [Description(XML_Constants.FIELD_DISPLAY_ENUMERATION)]
        Enumeration,
        Undefined
    }

    public enum RoleType
    {
        [Description(XML_Constants.ROLE_ADMIN)]
        Administrator,
        [Description(XML_Constants.ROLE_OWNER)]
        Owner,
        [Description(XML_Constants.ROLE_CONTRIB)]
        Contributor,
        [Description(XML_Constants.ROLE_READ_ALL)]
        ReadAll,
        [Description(XML_Constants.ROLE_ACL)]
        ACL,
        Undefined
    }

    public enum NPMDataType
    {
        Blob,
        String,
        SemiStr,
        Int32,
        Int64,
        Boolean,
        Date,
        Route,
        Undefined
    }

    //public enum FieldDataType
    //{
    //    String,
    //    DateTime,
    //    Int32,
    //    Int64,
    //    Boolean,
    //    Undefined
    //}

    public enum SearchOperationType
    {
        [Description(XML_Constants.BEGINS_WITH)]
        BEGINS_WITH,
        [Description(XML_Constants.CONTAINS)]
        CONTAINS,
        [Description(XML_Constants.DOES_NOT_CONTAIN)]
        DOES_NOT_CONTAIN,
        [Description(XML_Constants.ENDS_WITH)]
        ENDS_WITH,
        [Description(XML_Constants.EQUALS)]
        EQUALS,
        [Description(XML_Constants.GREATER_EQUAL)]
        GREATER_EQUAL,
        [Description(XML_Constants.GREATER_THAN)]
        GREATER_THAN,
        [Description(XML_Constants.LESS_EQUAL)]
        LESS_EQUAL,
        [Description(XML_Constants.LESS_THAN)]
        LESS_THAN,
        [Description(XML_Constants.NOT_EQUAL)]
        NOT_EQUAL,
        Undefined
    }

    internal static class Enum<T>
    {
        public static string EnumText(T value)
        {
            DescriptionAttribute[] da = (DescriptionAttribute[])(typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false));
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }

    #endregion
}
