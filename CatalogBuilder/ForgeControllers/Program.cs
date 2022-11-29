using Autodesk.Forge.Controllers;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeControllers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            oAuthController.Test();
            var t = Task.Run(async () =>
            {

                try
                {
                    DataManagementController dMController = new DataManagementController();
                    var response = await dMController.UploadObjectASync("C:\\Users\\ricej\\OneDrive - autodesk\\Samples\\iLogic\\Wheel_2021\\WheelAssembly.png",
                                                        oAuthController.Token);
                    string signedResponse = await dMController.GetSignedURLAsync(response, oAuthController.Token);
                    
                    //string signedUrl = signedDto.signedUrl;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
            t.Wait();

        }
    }
}
