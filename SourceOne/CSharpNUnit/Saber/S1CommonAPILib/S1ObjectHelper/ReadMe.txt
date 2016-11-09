Author:		Neil Wang
Folder:		S1ObjectManager
Description:
	This is the manager for the S1 objects.
	The S1ObjectFactory is the entrance to create the S1 objects, and user needs to offer the S1ObjectType to tell which kind of object he/she wants to create. For the S1ObjectType, we'll add the postfix "_TBD" to indicate that this kind of object is not support yet.
	For each kind of S1 object manager, it needs to offer below functionalities:
		Add without offering parameters, the object will be created using the default values
		Add by offering a S1 wrapper object, the object will be created using the value specified in the wrapper object if it exists. For the value not specified by the wrapper object, the default value will be used.
		Get All
		Get One 
		Delete if supported
		Update 
 