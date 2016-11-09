This is the base class for all NUnit test fixtures, all test fixtures should inherent the class. It offers below functionalities:
	The instance for the test data management(Messages/Tasks/Appointments ... for exchange/notes, the sites/pages ... for SharePoint ...)
	The instance for the test environment information management(The host, S1 configuration, etc.)
	The instance for the test parameters management(Manage the test parameters, this is mainly for the Data Driven Test)
	The integration with the Galaxy and Force project