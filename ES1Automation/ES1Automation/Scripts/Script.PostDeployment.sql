/*
	Create ATF Configuration
*/
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultTestCaseTimeout', N'60000', N'Default timeout value for test case')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultTestCaseRetryTimes', N'1', N'Default retrytimes value for test case')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultAutomationJobTimeout', N'7200000', N'Default timeout value for automation job')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultEnvironmentKeptTimeForPassedJob', N'86400000', N'Default period to keep the test environment whoes job is passed before discard it')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultEnvironmentKeptTimeForFailedJob', N'7200000', N'Default period to keep the test environment whoes job is failed before discard it')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultSMTPServer', N'10.98.38.25', N'Default SMTP server to send the result report')
INSERT INTO [dbo].[ATFConfiguration] ([ConfigName], [ConfigValue], [Description]) VALUES (N'DefaultAuthenticationDomain', N'corp.emc.com', N'Default LDAP service to authenticate the user of galaxy')

/*
	Create Basic Providers
*/

-- Environment Provider
INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description], [IsActive]) 
VALUES 
(
	0, 
	N'vCloud', 
	0,
	N'Core.Providers.EnvrionmentProviders.VCloudEnvironmentProvider', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
		<vCloudUrl>https://brs-dur-vmdvcd1.brs.lab.emc.com</vCloudUrl>
		<username>automation</username>
		<password>emcsiax@QA</password>
		<organization>Mozy</organization>
		<vdc>SourceOne-Mozy-OVDC1</vdc>
		<timeout>600000</timeout>
	 </config>', 
	N'vCloud in Lab',
	1
)


-- Environment Provider VCD_Shanghai_ES1
INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description], [IsActive]) 
VALUES 
(
	1, 
	N'VCD_Shanghai_ES1', 
	0,
	N'Core.Providers.EnvrionmentProviders.VCloudEnvironmentProvider', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
  <vCloudUrl>https://10.98.29.205/cloud/org/es1_qe_shanghai/#</vCloudUrl>
  <username>galaxy</username>
  <password>galaxy</password>
  <organization>es1_qe_shanghai</organization>
  <vdc>ES1_QE_Shanghai VDC</vdc>
  <timeout>600000</timeout>
</config>', 
	N'vcloud es1 qe',
	1
)


-- Build Provider
INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	2, 
	N'SourceOne Build Server', 
	1, 
	N'Core.Providers.BuildProviders.BuildFileServer', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
		<path>\\na-releng.otg.com\releng\builds</path>
		<username>otg\nwang</username>
		<password>emcsiax@QA</password>
		<product>SourceOne</product>
	</config>', 
	N'SourceOne Build Server',
	1
)
-- DPSearch Build Provider
INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	3, 
	N'DPSearchBuildServer', 
	1, 
	N'Core.Providers.BuildProviders.DPSearchBuildServer', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
  <path>\\na-releng.otg.com\releng\builds</path>
  <username>otg\nwang</username>
  <password>emcsiax@QA</password>
</config>', 
	N'Build Location',
	1
)


INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	4, 
	N'Supervisor Build Server', 
	1, 
	N'Core.Providers.BuildProviders.SupervisorWebBuildProvider', 
	N'C:\Personal\Projects\ES1.TFS\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
		<path>\\na-releng.otg.com\releng\builds</path>
		<username>otg\nwang</username>
		<password>emcsiax@QA</password>
		<product>Supervisor Web</product>
	 </config>', 
	N'Supervisor Build Server',
	1
)

-- TestCase Provider
INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	5, 
	N'ATFTestCaseProvider', 
	2, 
	N'Core.Providers.TestCaseProviders.ATFTestCaseProvider', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'', 
	N'ATF',
	1
)

INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	6, 
	N'SourceOne RQM Server', 
	2, 
	N'Core.Providers.TestCaseProviders.RQMTestCaseProvider', 
	N'C:\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
		<url>https://jazzapps.otg.com:9443/qm</url>
		<projectAlias>SourceOne+%28Quality+Management%29</projectAlias>
		<username>svc_s1auto</username>
		<password>emcsiax@QA</password>
	 </config>', 
	N'SourceOne RQM Server',
	1
)

INSERT [dbo].[Provider] ([ProviderId], [Name], [Category], [Type], [Path], [Config], [Description],[IsActive]) 
VALUES 
(
	7, 
	N'Supervisor RQM Server', 
	2, 
	N'Core.Providers.TestCaseProviders.RQMTestCaseProvider', 
	N'C:\Personal\Projects\ES1.TFS\AutomationFramework\ES1Automation\Main\ES1Automation\Core\bin\Debug\Core.dll', 
	N'<config>
		<url>https://jazzapps.otg.com:9443/qm</url>
		<projectAlias>Supervisor+%28Quality+Management%29</projectAlias>
		<username>svc_s1auto</username>
		<password>emcsiax@QA</password>
	 </config>', 
	N'Supervisor RQM Server',
	1
)


GO

/*
	Create Basic Users
*/

SET IDENTITY_INSERT [dbo].[User] ON

INSERT [dbo].[User] ([UserId], [Type], [Username], [Password], [Role], [IsActive], [Description]) VALUES (0, 0, N'automation', N'CgAAAB+LCAAAAAAABABLzU0uzkyscAh0BACQtE8NCgAAAA==', 0, 1, N'test system admin for automation')

SET IDENTITY_INSERT [dbo].[User] OFF

GO

/*
	Create Basic Product
*/

INSERT INTO [ES1Automation].[dbo].[Product] ([ProductId], [Name], [Description], [BuildProviderId], [TestCaseProviderId], [EnvironmentProviderId], [RunTime]) VALUES (0, N'Automation Framework', NULL, 1, 3, 0, N'CSharpNUnit')
INSERT INTO [ES1Automation].[dbo].[Product] ([ProductId], [Name], [Description], [BuildProviderId], [TestCaseProviderId], [EnvironmentProviderId], [RunTime]) VALUES (1, N'SourceOne', NULL, 1, 3, 0, N'CSharpNUnit')
INSERT INTO [ES1Automation].[dbo].[Product] ([ProductId], [Name], [Description], [BuildProviderId], [TestCaseProviderId], [EnvironmentProviderId], [RunTime]) VALUES (2, N'Common Index Search', NULL, 1, 3, 0, N'RubyMiniTest')
INSERT INTO [ES1Automation].[dbo].[Product] ([ProductId], [Name], [Description], [BuildProviderId], [TestCaseProviderId], [EnvironmentProviderId], [RunTime]) VALUES (3, N'Data Protection Search', NULL, 1, 3, 0, N'RubyMiniTest')

GO

/*
	Create Basic project
*/
SET IDENTITY_INSERT [dbo].[project] ON

INSERT INTO [ES1Automation].[dbo].[project] ([ProjectId], [Name], [Description], [VCSType], [VCSServer], [VCSUser], [VCSPassword], [VCSRootPath], [RunTime]) VALUES (0, N'Automation Framework', NULL, 1, N'http://10.37.11.121:8080/tfs/defaultcollection', N'es1\wangn6', N'wangn6', N'AutomationFramework/ES1Automation/Main/Saber', N'CSharpNUnit')
INSERT INTO [ES1Automation].[dbo].[project] ([ProjectId], [Name], [Description], [VCSType], [VCSServer], [VCSUser], [VCSPassword], [VCSRootPath], [RunTime]) VALUES (1, N'DPSearch1.0', N'DPSearch1.0', 2, N'http://10.98.17.63:16080/ccrc', N'otg\liuyu', N'7ujm*IK<7', N'DPSearch/src/production/automation', N'0')

SET IDENTITY_INSERT [dbo].[project] OFF

GO

/*
	Build Test Cases Tree
*/

SET IDENTITY_INSERT [dbo].[TestSuite] ON
-- LEVEL 1: root

INSERT INTO [ES1Automation].[dbo].[TestSuite]([SuiteId],[ProviderId],[Type],[Name],[SubSuites],[TestCases],[CreateBy],[CreateTime],[ModityBy],[ModifyTime],[Description],[IsActive])
       VALUES (0, NULL, 0,'All Test Cases', NULL, NULL, 0, GETDATE(), 0, GETDATE(), NULL, 1)

INSERT INTO [ES1Automation].[dbo].[TestSuite]([SuiteId],[ProviderId],[Type],[Name],[SubSuites],[TestCases],[CreateBy],[CreateTime],[ModityBy],[ModifyTime],[Description],[IsActive])
       VALUES (1000, NULL, 3,'Customized Test Suites', NULL, NULL, 0, GETDATE(), 0, GETDATE(), NULL, 1)

INSERT INTO [ES1Automation].[dbo].[TestSuite]([SuiteId],[ProviderId],[Type],[Name],[SubSuites],[TestCases],[CreateBy],[CreateTime],[ModityBy],[ModifyTime],[Description],[IsActive])
       VALUES (2000, NULL, 0,'RQM Test Suites', NULL, NULL, 0, GETDATE(), 0, GETDATE(), NULL, 1)

INSERT INTO [ES1Automation].[dbo].[TestSuite]([SuiteId],[ProviderId],[Type],[Name],[SubSuites],[TestCases],[CreateBy],[CreateTime],[ModityBy],[ModifyTime],[Description],[IsActive])
       VALUES (3000, NULL, 0,'RQM Test Plans', NULL, NULL, 0, GETDATE(), 0, GETDATE(), NULL, 1)

SET IDENTITY_INSERT [dbo].[TestSuite] OFF

GO

/*
	Create a build which stands for the latest mainline build
*/

SET IDENTITY_INSERT [dbo].[Build] ON

INSERT INTO ES1Automation.dbo.Build([BuildId] ,[ProviderId] ,[ProductId] ,[Name] ,[Type] ,[Status] ,[BranchId] ,[ReleaseId], [Number] ,[Location] ,[Description])
        VALUES( 0, 1, 0, 'Latest build', 0, 0, NULL, NULL, '0.0.0', 'Fake location', 'The place holder for the latest build')

SET IDENTITY_INSERT [dbo].[Build] OFF

GO
