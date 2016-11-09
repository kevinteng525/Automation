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
    internal interface IGOSPropertyDescriptor
    {
        PresentationProperty AssociatedProperty { set; get;}
    }
}
