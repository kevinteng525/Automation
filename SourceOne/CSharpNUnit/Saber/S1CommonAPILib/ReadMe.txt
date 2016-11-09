Author:		Neil Wang
Project:	S1CommonAPILib
Description:
	This is the project which acts as the middle layer between the S1 and the automation test. 
	Ideally, user doesn't need to include any S1 interop dlls in the automation test project, all native S1 API calls are from the LIB. 
	It wraps the common API for S1(S1ObjectHelper) and offers the user-friendly object for users to specify the parameters needed.
	