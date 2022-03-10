1. Download the BoldReportServer application build version 3.3.23(https://www.boldreports.com/account/downloads) and BoldBI application build version 4.2.69(https://www.boldbi.com/account/downloads).
2. Install  the Bold BI and confirgure the application(https://help.boldbi.com/embedded-bi/setup/deploying-in-windows/installation-and-deployment/) and setup an application(https://help.boldbi.com/embedded-bi/application-startup/latest/).
3. Install the BoldReportServer application(https://help.boldreports.com/enterprise-reporting/administrator-guide/installation/windows-installer/) and while installing the BoldReportServer. It will ask confirmation for Common Login(https://help.boldreports.com/enterprise-reporting/administrator-guide/installation/windows-installer/#bold-bi-with-bold-reports-enterprise-reporting-edition-installation) and Click on yes.
4. Once Installation is gets over. Click Settings option in IDP application.
5. Go to Manage License section and click Login to activate account under Enterprise Reporting option.
6. Once the account has been activated, go to site page and create a site(https://help.boldreports.com/enterprise-reporting/administrator-guide/manage-tenants/create-site/).
7. Once downloaded the source code, open the solution in the Visual studio. 
8. Open the embedDetails.json file in the below location,
/App_Data/default/embedDetails.json

9. Please change the below properties as per your Bold Reports Server,

| ReportRootUrl       | Bold Reports Server URL (ex: http://localhost:5000/reporting, http://demo.boldreports.com/reporting)                                                                                                                                    |
|---------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| SiteIdentifier      | For Bold BI Enterprise edition, it should be like `site/{tenant-name}`. For Bold BI Cloud, it should be empty string.                                                                                                                   |
| ReportSiteIdentifier| For  Bold Reports Enterprise edition, it should be like `site/{tenant-name}`. For Bold Reports Cloud, it should be empty string.                                                                                                        |
| Environment         | Your Bold Reports application environment. (If Cloud, you should use `cloud`, if Enterprise, you should use `enterprise`)                                                                                                               |
| ReportEmail         | UserEmail of the Admin in your Bold Reports Server, which would be used to get the reports list                                                                                                                                         |
| Email               | UserEmail of the Admin in your Bold BI, which would be used to get the dashboards list                                                                                                                                                  |
| DisplayName         | This is the name displayed in the embedded application.                                                                                                                                                                                 |
| EmbedSecret         | You could get your EmbedSecret key from Embed tab by enabling `Enable embed authentication` in Administration page https://help.boldbi.com/embedded-bi/site-administration/embed-settings/                                              |
| ReportEmbedSecret   | You could get your ReportEmbedSecret key from Embed tab by enabling `Enable embed authentication` in Administration page https://help.boldreports.com/enterprise-reporting/developer-guide/embed-in-application/iframe/embed-secret-key/|
| Serverurl           | Dashboard Server BI URL (ex: http://localhost:5000/bi, http://demo.boldbi.com/bi)                                                                                                                                                       |
| ReportServerurl     | Bold Reports Server  URL (ex: http://localhost:5000/reporting, http://demo.boldreports.com/reporting)                                                                                                                                   |
| RootUrl             | Dashboard Server BI URL (ex: http://localhost:5000/bi, http://demo.boldbi.com/bi)                                                                                                                                                       |

10. please Download and extracted packages link is mentioned (https://www.syncfusion.com/downloads/support/directtrac/general/ze/api573703020) and then replaces the dll files into the {BuildInstalledLocation}\BoldServices\reporting\api path and restart the application in IIS Manager.