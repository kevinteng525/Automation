Author:		Neil Wang
Time:		2014.03.04
Descrption:	
			This is an example about how to write a test case using our Saber infrastructure.
			1. There must be two folders under this project, 
				one is "TestData" which will store the test data that you need for your testing;
				one is "TestMetadata" which contains the parameters that will be used for your testing.
			2. The test class must be inherit the base class "TestClassBase" and call the default constructor of base class to do the initialization work.
			3. The test data and test metadata files should be copied to the output directory always
