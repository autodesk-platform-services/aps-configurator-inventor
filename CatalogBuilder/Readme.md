# CatalogBuilder Sample

Sample **Inventor** add-in that compliments the **Configurator Web App** by providing functionality to push **Inventor** assemblies and their configuration options to the Web App from inside **Inventor**. 

## How to run this sample
1) First make sure that the **Configurator Web App** is already working fine and is running - i.e. you completed the setps outlined in [Run sample for the first time](../README.md#run-sample-for-the-first-time) 

1) Set APS credentials, APS_CLIENT_ID and APS_CLIENT_SECRET, in `\ForgeControllers\Controllers\oAuthController.cs`.

1) Check the exact name of the bucket that the **Configurator Web App** generated on OSS and set the value of `bucketId` in `\ForgeControllers\Controllers\DataManagementController.cs` to that.
You can use a tool like [Buckets Tools](https://oss-manager.autodesk.io/) to do that.

1) In the **Project Properties** of **CatalogBuilder** on the **Debug** tab, make sure **Output path** is set to `C:\ProgramData\Autodesk\ApplicationPlugins\CatalogBuilder`.

1) Build the solution

1) Run the **CatalogBuilder** project  

## Info
Language/Compiler: VC# (.NET)
Server: Inventor.
