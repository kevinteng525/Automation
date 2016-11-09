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

namespace Saber.S1CommonAPILib.S1SearchWrapper
{
    public delegate void LogInformation(string message);

    public delegate void LogVerbose(string message);

    public delegate void LogException(Exception ex);

    public interface IGOSPresentationModel
    {
        event LogInformation OnLogInformation;
        
        event LogVerbose OnLogVerbose;
        
        event LogException OnLogException;

        List<PresentationProperty> PresentationPropertyList { get; }

        List<PresentationObject> PresentationObjectList { get; }

        List<PresentationGroup> PresentationGroupList { get; }

        PresentationProperty this[int npmPropId, int npmObjectId] { get; }

        PresentationProperty this[int npmPropId] { get; }

        PresentationObject this[string objectId] { get; }

        string RootImagePath { get; set; }

        string Name { get; }

        string Version { get; }

        PresentationProperty GetProperty(int npmPropId, int npmObjectId);

        PresentationProperty GetProperty(int npmPropId);

        PresentationObject GetObject(string objectId);

        PresentationProperty GetPresentationOnlyProperty(string presentationPropId);

        GroupingItem GetPresentationGroup(string ObjectID);

        bool IsXmlValid();

        bool ContainsObject(PresentationObject obj);

        bool ContainsProperty(PresentationProperty prop);

        void LoadFromNpm(string es1NpmXml, Dictionary<string, string[]> enumMappings);

        void LoadFromPxml(string es1NpmXml, string pXml);

        void LoadExtensibility(string extXml);

        void Save(string fileName);

        void Update(string pXmlUpdated);

        string ToXml();
    }
}
