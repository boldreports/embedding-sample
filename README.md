Once downloaded the source code, open the solution in the Visual studio. 
Open the embedDetails.json file in the below location,
/App_Data/default/embedDetails.json
 

Please change the below properties as per your Bold Reports Server,

| ReportRootUrl  | Bold Reports Server URL (ex: http://localhost:5000/reporting, http://demo.boldreports.com/reporting)                                                                                                                               |
|----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| SiteIdentifier | For  Bold Reports Enterprise edition, it should be like `site/site1`. For Bold Reports Cloud, it should be empty string.                                                                                                           |
| Environment    | Your Bold Reports application environment. (If Cloud, you should use `cloud`, if Enterprise, you should use `enterprise`)                                                                                                          |
| ReportEmail    | UserEmail of the Admin in your Bold Reports Server, which would be used to get the reports list                                                                                                                                    |
| Email          | The email of the user who is present in the Bold BI Server.                                                                                                                                                                        |
| DisplayName    | This is the name displayed in the embedded application.                                                                                                                                                                            |
| EmbedSecret    | You could get your EmbedSecret key from Embed tab by enabling `Enable embed authentication` in Administration page https://help.boldreports.com/enterprise-reporting/developer-guide/embed-in-application/iframe/embed-secret-key/ |
| Serverurl      | Bold Reports Server  URL (ex: http://localhost:5000/reporting, http://demo.boldreports.com/reporting)                                                                                                                              |
| RootUrl        | Dashboard Server BI URL (ex: http://localhost:5000/bi, http://demo.boldbi.com/bi)                                                                                                                                                  |

please Download and extracted packages link is mentioned (https://www.syncfusion.com/downloads/support/directtrac/general/ze/api573703020) and then replaces the dll files into the {BuildInstalledLocation}\BoldServices\reporting\api path and restart the application in IIS Manager.