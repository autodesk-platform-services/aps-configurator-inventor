/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using System;
using System.Threading.Tasks;

namespace Autodesk.Forge.Controllers
{
	public static class oAuthController
	{
		// Initialize the oAuth 2.0 client configuration fron enviroment variables
		// you can also hardcode them in the code if you want in the placeholders below
		private static string APS_CLIENT_ID = "<your client id>";
		//Environment.GetEnvironmentVariable("APS_CLIENT_ID") ?? "your_client_id";
		private static string APS_CLIENT_SECRET = "<your client secret>";
		//Environment.GetEnvironmentVariable("APS_CLIENT_SECRET") ?? "your_client_secret";
		private static Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite };

		// Intialize the 2-legged oAuth 2.0 client.
		private static TwoLeggedApi _twoLeggedApi = new TwoLeggedApi();
        

		public static string Token
		{
			get {
				dynamic bearer = oAuthController._2leggedSynchronous();
				if (bearer == null)
				{
					Console.WriteLine("You were not granted a new access_token!");
					return "";
				}
				// The call returned successfully and you got a valid access_token.
				return  bearer.token_type + " " + bearer.access_token;
			}
		}


		// Synchronous example
		internal static dynamic _2leggedSynchronous()
		{
			try
			{
				// Call the synchronous version of the 2-legged client with HTTP information
				// HTTP information will help you to verify if the call was successful as well
				// as read the HTTP transaction headers.
				ApiResponse<dynamic> bearer = _twoLeggedApi.AuthenticateWithHttpInfo(APS_CLIENT_ID, APS_CLIENT_SECRET, oAuthConstants.CLIENT_CREDENTIALS, _scope);
				if (bearer.StatusCode != 200)
					throw new Exception("Request failed! (with HTTP response " + bearer.StatusCode + ")");

				// The JSON response from the oAuth server is the Data variable and has been
				// already parsed into a DynamicDictionary object.

				return (bearer.Data);
			}
			catch (Exception /*ex*/ )
			{
				return (null);
			}
		}

		public static void Test()
		{
			dynamic bearer = _2leggedSynchronous();
			if (bearer == null)
			{
				Console.WriteLine("You were not granted a new access_token!");
				return;
			}
			// The call returned successfully and you got a valid access_token.
			string token = bearer.token_type + " " + bearer.access_token;
			Console.WriteLine("Your synchronous token test is: " + token);
			// ...
		}

		// Asynchronous example (recommended)
		internal static async Task<dynamic> _2leggedAsync()
		{
			try
			{
				// Call the asynchronous version of the 2-legged client with HTTP information
				// HTTP information will help you to verify if the call was successful as well
				// as read the HTTP transaction headers.
				ApiResponse<dynamic> bearer = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(APS_CLIENT_ID, APS_CLIENT_SECRET, oAuthConstants.CLIENT_CREDENTIALS, _scope);
				//if ( bearer.StatusCode != 200 )
				//	throw new Exception ("Request failed! (with HTTP response " + bearer.StatusCode + ")") ;

				// The JSON response from the oAuth server is the Data variable and has been
				// already parsed into a DynamicDictionary object.

				return (bearer.Data);
			}
			catch (Exception /*ex*/ )
			{
				return (null);
			}
		}

	}
}

