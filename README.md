1. Download the BoldReportServer application build version 3.3.23(https://www.boldreports.com/account/downloads) and BoldBI application build version 4.2.69(https://www.boldbi.com/account/downloads).
2. Install and configure Bold BI application. Please find the below help links.
   https://help.boldbi.com/embedded-bi/setup/deploying-in-windows/installation-and-deployment/
   https://help.boldbi.com/embedded-bi/application-startup/latest/
3. Install the BoldReportServer application as common login. Please find the below help link.
   https://help.boldreports.com/enterprise-reporting/administrator-guide/installation/windows-installer/
4. Once Installation is done. Navigate to IDP application and go to settings page
5. Go to Manage License section and click Login to activate account under Enterprise Reporting option.
6. Once the account has been activated, go to site page and url be like {http://localhost:5000/ums/sites} and create a site for Bold Report server. Please find the below help link.
   https://help.boldreports.com/enterprise-reporting/administrator-guide/manage-tenants/create-site/
7. Once downloaded the source code, open the solution in the Visual studio. 
8. Edit the embedDetails.json file in the below location,
   /App_Data/default/embedDetails.json
9. Please change the below properties as per your Bold Reports Server,

| ReportRootUrl       | provide your Bold Reports Server URL (ex: http://localhost:5000/reporting, http://demo.boldreports.com/reporting)                                                                                                                                    |
|---------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Serverurl           | provide your Dashboard Server BI URL (ex: http://localhost:5000/bi/site/{tenant-name}, http://demo.boldbi.com/bi/site/{tenant-name})                                                                                                                                                       |
| ReportServerurl     | provide your Bold Reports Server  URL (ex: http://localhost:5000/reporting/site/{tenant-name}, http://demo.boldreports.com/reporting/site/{tenant-name})                                                                                                                                   |
| SiteIdentifier      | Provide your Bold BI site identifier it should be like `site/{tenant-name}`. For eg:site/site1.                                                                                                                   |
| ReportSiteIdentifier| Provide your  Bold Reports server site identifier, it should be like `site/{tenant-name}`. For eg:site/site1.                                                                                                       |
| Environment         | Your Bold Reports application environment. It should be `on-premise`.                                                                                                               |
| ReportEmail         | Provide your admin email address of the Bold Report server. For eg: demo.boldreports.com                                                                                                                                         |
| Email               | Provide your admin email address of the Bold BI. For eg: demo.boldbi.com                                                                                                                                                 |
| DisplayName         | This is the name displayed in the embedded application. you can provide any name.                                                                                                                                                                                 |
| EmbedSecret         | provide embed secret key of Bold BI. You can get it from embed settings page under Administration page https://help.boldbi.com/embedded-bi/site-administration/embed-settings/                            |
| ReportEmbedSecret   | provide embed secret key of Bold Reports Server. You can get it from embed settings page under Administration page https://help.boldreports.com/enterprise-reporting/developer-guide/embed-in-application/iframe/embed-secret-key/|
| RootUrl             | provide your Dashboard Server BI URL (ex: http://localhost:5000/bi, http://demo.boldbi.com/bi)                                                                                                                                                       |

10. please Download and extracted packages link is mentioned (https://www.syncfusion.com/downloads/support/directtrac/general/ze/api573703020) and then replaces the dll files into the {BuildInstalledLocation}\BoldServices\reporting\api path and restart the application in IIS Manager.