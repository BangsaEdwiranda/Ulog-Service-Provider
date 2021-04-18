# BizCover MVC app

TODO: add project description and other imporatnt details

## Local setup

### Code (.NET + node.js)

1. Clone the repository (for example to `C:\Code\BizCoverMvc`)
2. Install NPM packages
    ```
    pushd C:\Code\BizCoverMvc
    npm install -g gulp
    npm install -g bower
    npm install tsd@next -g
    ```
3. Open Solution in Visual Studio
4. Ensure `dev:watch` task is running

    * Open *Task Runner Explorer*<br />
    ![Task Runner Explorer](/../github-images/.github-images/readme.me/TaskRunnerExplorer.png?raw=true)
    * Check the task<br />
    ![Task running](/../github-images/.github-images/readme.me/TaskRunning.png?raw=true)
5. Build the solution in Visual Studio

6. Client-side build used to be part of VS Build but has been disabled in Debug mode to speed up development build time. Now you have to build it manually when necessary:
    * Open a CLI (e.g. cmd, cmder).
    * Go to BizcoverMvc project.
    * First time, you need to run **npm install** to install gulp.
    * Run **gulp bundle**. This will do everything you need (e.g.  install, clean, bundle, etc.)
    * Refer to *gulpfile.js* for more details.

7. Set the database connection string to the relevant location<br />
    C:\Code\BizCoverMvc\BizCoverMvc\Web.config:    
    ```
    <add name="BizCoverConnectionString" connectionString="Server=localhost\SQLEXPRESS;Database=local-insure.bizcover.com.au;User Id=sa;Password=password" providerName="System.Data.SqlClient" />
    ```

8. For reading/writing PDF docs to AWS S3 ensure your AWS credentials file (located in C:\Users\\<user_name>\\.aws\credentials) is set up with your own access/secret key and the NonProduction role. Refer to [AWS User Setup](https://bizcover.atlassian.net/wiki/spaces/DEVOPS/pages/761135682/Identity%2BAccount%2BUser%2BAccess) for instructions.

### IIS Setup

1. Follow the steps above to build the app
2. Create a new Website (not new Application under website) in IIS with a .Net 4.0 app pool (any will do). Use a different port to 80 thats not currently in use.
   * Optionally ensure [IIS URL Rewrite](https://www.iis.net/downloads/microsoft/url-rewrite) is correctly setup otherwise you will get a 500.19 error (The requested page cannot be accessed because the related configuration data for the page is invalid)
   * Alternatively edit web.config and comment out `<rewrite>` section

### Database

1. Refer to [BizCover.DatabaseCleanup.Scripts](https://github.com/BizCover/BizCover.DatabaseCleanup.Scripts) for installation instructions.

## Testing whitelabels

Refer to [Wiki/How to test whitelabels locally](https://github.com/BizCover/BizCoverMvc/wiki/How-to-test-whitelabels-locally)


## Troubleshooting

Refer to [Wiki/Troubleshooting section](https://github.com/BizCover/BizCoverMvc/wiki/Troubleshooting)

## Process Notes
For any code-merge to master, please ensure that it has a corresponding JIRA ticket in the corresponding BAU Board https://bizcover.atlassian.net/secure/RapidBoard.jspa?projectKey=BAU&rapidView=72 and/or https://bizcover.atlassian.net/secure/RapidBoard.jspa?rapidView=18

Once the code is merged to master, please update the Fix Version of the ticket to "master"
