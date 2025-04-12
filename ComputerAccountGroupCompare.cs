// Copyright (C) 2025 Akil Woolfolk Sr. 
// All Rights Reserved
// All the changes released under the MIT license as the original code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Management;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;
using SystemsAdminPro.Utility;

namespace ComputerAccountGroupCompare
{
    class CAGCMain
    {
        struct CMDArguments
        {
            public string strReferenceComputer;
            public string strCheckComputer;
            public bool bParseCmdArguments;
        }

        static void funcPrintParameterSyntax()
        {
            Console.WriteLine("ComputerAccountGroupCompare");
            Console.WriteLine();
            Console.WriteLine("Parameter syntax:");
            Console.WriteLine();
            Console.WriteLine("Use the following parameters:");
            Console.WriteLine("-run                              required parameter");
            Console.WriteLine("-ref:[ComputerName]               specify reference computer");
            Console.WriteLine("-check:[ComputerName]             specify computer to check");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("ComputerAccountGroupCompare -run -ref:COMPUTER1 -check:COMPUTER2");
        }

        static void funcGetFuncCatchCode(string strFunctionName, Exception currentex)
        {
            string strCatchCode = "";

            Dictionary<string, string> dCatchTable = new Dictionary<string, string>();
            dCatchTable.Add("funcGetFuncCatchCode", "f0");
            dCatchTable.Add("funcPrintParameterWarning", "f2");
            dCatchTable.Add("funcPrintParameterSyntax", "f3");
            dCatchTable.Add("funcParseCmdArguments", "f4");
            dCatchTable.Add("funcProgramExecution", "f5");
            dCatchTable.Add("funcProgramRegistryTag", "f6");
            dCatchTable.Add("funcCreateDSSearcher", "f7");
            dCatchTable.Add("funcCreatePrincipalContext", "f8");
            dCatchTable.Add("funcCheckNameExclusion", "f9");
            dCatchTable.Add("funcMoveDisabledAccounts", "f10");
            dCatchTable.Add("funcFindAccountsToDisable", "f11");
            dCatchTable.Add("funcCheckLastLogin", "f12");
            dCatchTable.Add("funcRemoveUserFromGroup", "f13");
            dCatchTable.Add("funcToEventLog", "f14");
            dCatchTable.Add("funcCheckForFile", "f15");
            dCatchTable.Add("funcCheckForOU", "f16");
            dCatchTable.Add("funcWriteToErrorLog", "f17");

            if (dCatchTable.ContainsKey(strFunctionName))
            {
                strCatchCode = "err" + dCatchTable[strFunctionName] + ": ";
            }

            //[DebugLine] Console.WriteLine(strCatchCode + currentex.GetType().ToString());
            //[DebugLine] Console.WriteLine(strCatchCode + currentex.Message);

            Construct.WriteToErrorLogFile(strCatchCode + currentex.GetType().ToString());
            Construct.WriteToErrorLogFile(strCatchCode + currentex.Message);

        }

        static CMDArguments funcParseCmdArguments(string[] cmdargs)
        {
            CMDArguments objCMDArguments = new CMDArguments();

            try
            {
                bool bCmdArg1Complete = false;

                if (cmdargs[0] == "-run" & cmdargs.Length > 1)
                {
                    if (cmdargs[1].Contains("-ref:"))
                    {
                        // [DebugLine] Console.WriteLine(cmdargs[1].Substring(5));
                        objCMDArguments.strReferenceComputer = cmdargs[1].Substring(5);
                        bCmdArg1Complete = true;

                        if (bCmdArg1Complete & cmdargs.Length > 2)
                        {
                            if (cmdargs[2].Contains("-check:"))
                            {
                                // [DebugLine] Console.WriteLine(cmdargs[2].Substring(7));
                                objCMDArguments.strCheckComputer = cmdargs[2].Substring(7);
                                objCMDArguments.bParseCmdArguments = true;
                            }
                        }
                    }
                }
                else
                {
                    objCMDArguments.bParseCmdArguments = false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                objCMDArguments.bParseCmdArguments = false;
            }

            return objCMDArguments;
        }

        static void funcCompareComputerAccountGroups(CMDArguments objCMDArguments)
        {
            try
            {
                PrincipalContext ctxDomain = Construct.CreateDomainPrincipalContext();

                ComputerPrincipal computer1 = new ComputerPrincipal(ctxDomain);
                ComputerPrincipal computer2 = new ComputerPrincipal(ctxDomain);

                PrincipalSearcher ps1 = new PrincipalSearcher(computer1);
                PrincipalSearcher ps2 = new PrincipalSearcher(computer2);

                computer1.Name = objCMDArguments.strReferenceComputer;
                computer2.Name = objCMDArguments.strCheckComputer;

                // Tell the PrincipalSearcher what to search for.
                ps1.QueryFilter = computer1;
                ps2.QueryFilter = computer2;

                // Run the query. The query locates users 
                // that match the supplied user principal object. 
                Principal newPrincipal1 = ps1.FindOne();
                Principal newPrincipal2 = ps2.FindOne();

                if (newPrincipal1 != null & newPrincipal2 != null)
                {
                    computer1 = (ComputerPrincipal)newPrincipal1;
                    computer2 = (ComputerPrincipal)newPrincipal2;

                    Console.WriteLine("Computers: {0}, {1}", computer1.Name, computer2.Name);

                    PrincipalSearchResult<Principal> psr1 = computer1.GetGroups();
                    PrincipalSearchResult<Principal> psr2 = computer2.GetGroups();

                    Console.WriteLine("Number of groups for {0}: {1}", computer1.Name, psr1.Count<Principal>().ToString());
                    Console.WriteLine("Number of groups for {0}: {1}", computer2.Name, psr2.Count<Principal>().ToString());

                    //int intGroupsHigh = 0;
                    //int intComputer1GroupCount = 0;
                    //int intComputer2GroupCount = 0;

                    //intComputer1GroupCount = Int32.Parse(psr1.Count<Principal>().ToString());
                    //intComputer2GroupCount = Int32.Parse(psr2.Count<Principal>().ToString());

                    //if (intComputer1GroupCount > intComputer2GroupCount)
                    //{
                    //    intGroupsHigh = intComputer1GroupCount;
                    //}
                    //else
                    //{
                    //    intGroupsHigh = intComputer2GroupCount;
                    //}

                    Console.WriteLine();

                    //List<string> lstOutputLines = new List<string>();

                    //string strOutputLine = "";
                    //strOutputLine = "Groups: " + computer1.Name + "\t\t Groups: " + computer2.Name;
                    //lstOutputLines.Add(strOutputLine);
                    //strOutputLine = "------- \t\t -------";
                    //lstOutputLines.Add(strOutputLine);

                    //foreach (string strConsoleOut in lstOutputLines)
                    //{
                    //    Console.WriteLine(strConsoleOut);
                    //}
                }
                else
                {
                    Console.WriteLine("One of the computers could not be found.");
                    Console.WriteLine("Please check the parameters.");
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }
        }

        static void funcCompareGroups(CMDArguments objCMDArguments)
        {
            try
            {
                // [ Comment] Search filter strings for DirectorySearcher object filter
                string strQueryFilter1 = "";
                string strQueryFilter2 = "";
                string strFilterPrefix = "(&(objectclass=computer)(name=";
                string strFilterSuffix = "))";
                string strSourceAccount = "";
                string strDestinationAccount = "";

                strSourceAccount = objCMDArguments.strReferenceComputer;
                strDestinationAccount = objCMDArguments.strCheckComputer;
                strQueryFilter1 = strFilterPrefix + strSourceAccount + strFilterSuffix;
                strQueryFilter2 = strFilterPrefix + strDestinationAccount + strFilterSuffix;

                DirectorySearcher objAccountObjectSearcher1 = Construct.CreateRootDSSearcher();
                DirectorySearcher objAccountObjectSearcher2 = Construct.CreateRootDSSearcher();
                // [DebugLine]Console.WriteLine(objAccountObjectSearcher.SearchRoot.Path);

                // [Comment] Add filter to DirectorySearcher object
                objAccountObjectSearcher1.Filter = (strQueryFilter1);
                objAccountObjectSearcher2.Filter = (strQueryFilter2);
                objAccountObjectSearcher1.PropertiesToLoad.Add("memberOf");
                objAccountObjectSearcher2.PropertiesToLoad.Add("memberOf");

                // [Comment] Execute query, return results, display values
                SearchResult objAccountResult1 = objAccountObjectSearcher1.FindOne();
                // [DebugLine] Console.WriteLine("FindOne: " + objAccountResult1DE.Name);

                SearchResult objAccountResult2 = objAccountObjectSearcher2.FindOne();
                // [DebugLine] Console.WriteLine("FindOne: " + objAccountResult2DE.Name);

                if (objAccountResult1 != null & objAccountResult2 != null)
                {
                    DirectoryEntry objAccountResult1DE = objAccountResult1.GetDirectoryEntry();
                    DirectoryEntry objAccountResult2DE = objAccountResult2.GetDirectoryEntry();

                    string objAccountNameValue;
                    int intStrPosFirst = 3;
                    int intStrPosLast;

                    string strGroupName;

                    intStrPosLast = objAccountResult1DE.Name.Length;
                    objAccountNameValue = objAccountResult1DE.Name.Substring(intStrPosFirst, intStrPosLast - intStrPosFirst);

                    // [DebugLine] Console.WriteLine(objAccountNameValue);

                    if (objAccountResult1DE.Properties["memberOf"].Count > 0)
                    {
                        // [DebugLine] Console.WriteLine("Number of groups: " + objAccountResult1DE.Properties["memberOf"].Count.ToString());
                        Console.WriteLine("Number of groups for {0}: {1}",
                                          objAccountResult1DE.Name.Substring(intStrPosFirst, intStrPosLast - intStrPosFirst),
                                          objAccountResult1DE.Properties["memberOf"].Count.ToString());
                        Console.WriteLine("Number of groups for {0}: {1}",
                                          objAccountResult2DE.Name.Substring(intStrPosFirst, intStrPosLast - intStrPosFirst),
                                          objAccountResult2DE.Properties["memberOf"].Count.ToString());
                        Console.WriteLine();

                        bool isAccount2InGroup = false;

                        for (int propcounter = 0; propcounter < objAccountResult1DE.Properties["memberOf"].Count; propcounter++)
                        {
                            isAccount2InGroup = false;
                            strGroupName = (string)objAccountResult1DE.Properties["memberOf"][propcounter];
                            // [DebugLine] Console.WriteLine(strGroupName);
                            try
                            {
                                DirectoryEntry group = new DirectoryEntry("LDAP://" + strGroupName);
                                // [DebugLine] Console.WriteLine("Number of group members: " + group.Properties["member"].Count.ToString());
                                // [DebugLine] int intmembercounter = 0;

                                foreach (object o in group.Properties["member"])
                                {
                                    // [DebugLine] Console.WriteLine("membercounter: " + intmembercounter.ToString());
                                    // [DebugLine] Console.WriteLine(o.ToString());
                                    // [DebugLine] Console.WriteLine("**Member: " + o.ToString());
                                    // [DebugLine] Console.WriteLine("**Acct2Name: " + objAccountResult2DE.Name);
                                    // [DebugLine] Console.WriteLine("**Acct2Path: " + objAccountResult2DE.Path.Substring(7, objAccountResult2DE.Path.Length-7));
                                    if (objAccountResult2DE.Path.Substring(7, objAccountResult2DE.Path.Length - 7) == o.ToString())
                                    {
                                        isAccount2InGroup = true;
                                    }

                                    // [DebugLine] intmembercounter++;
                                }

                                if (isAccount2InGroup)
                                {
                                    //Console.WriteLine("{0} is a member of this group: {1}", strDestinationAccount, group.Name.Substring(3, group.Name.Length - 3));
                                    Console.WriteLine("{0} group {1}: {2} is a member", strSourceAccount, group.Name.Substring(3, group.Name.Length - 3), strDestinationAccount);
                                    // [DebugLine] Console.WriteLine(group.Path);
                                    // [DebugLine] Console.WriteLine(">>>if: isAccount2InGroup");
                                    //Console.WriteLine();
                                }
                                else
                                {
                                    //Console.WriteLine("{0} is not a member of this group: {1}", strDestinationAccount, group.Name.Substring(3, group.Name.Length - 3));
                                    Console.WriteLine("{0} group {1}: {2} is NOT a member", strSourceAccount, group.Name.Substring(3, group.Name.Length - 3), strDestinationAccount);
                                    // [DebugLine] Console.WriteLine(group.Path);
                                    // [DebugLine] Console.WriteLine(">>>else: isAccount2InGroup");
                                    // [DebugLine] Console.WriteLine();
                                    // [DebugLine] string tmpPath = objAccountResult2DE.Path;
                                    // [DebugLine] Console.WriteLine(tmpPath);
                                }

                                // [DebugLine] Console.WriteLine(group.Properties["member"].Count.ToString());

                                if (group == null)
                                {
                                    Console.WriteLine("group directoryentry was not created");
                                }
                            }
                            catch (Exception ex)
                            {
                                MethodBase mb1 = MethodBase.GetCurrentMethod();
                                funcGetFuncCatchCode(mb1.Name, ex);
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("One or both of the computers could not be found.");
                    Console.WriteLine("Please check the parameters.");
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }
        }

        static void funcProgramExecution(CMDArguments objCMDArguments2)
        {
            try
            {
                  funcCompareGroups(objCMDArguments2);               
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Construct.strProgramName = "ComputerAccountGroupCompare";

                if (args.Length == 0)
                {
                    Construct.PrintParameterWarning(Construct.strProgramName);
                }
                else
                {
                    if (args[0] == "-?")
                    {
                        funcPrintParameterSyntax();
                    }
                    else
                    {
                        string[] arrArgs = args;
                        CMDArguments objArgumentsProcessed = funcParseCmdArguments(arrArgs);

                        if (objArgumentsProcessed.bParseCmdArguments)
                        {
                            funcProgramExecution(objArgumentsProcessed);
                        }
                        else
                        {
                            Construct.PrintParameterWarning(Construct.strProgramName);
                        } // check objArgumentsProcessed.bParseCmdArguments
                    } // check args[0] = "-?"
                } // check args.Length == 0
            }
            catch (Exception ex)
            {
                Console.WriteLine("errm0: {0}", ex.Message);
            }
        } // Main
    }
}
