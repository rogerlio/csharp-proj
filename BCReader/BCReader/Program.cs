﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BCReader
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().EndsWith(".xml"))
                {
                    doFile(args[i]);
                }
            }
            if (args.Length == 0)
            {
                string strFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
                string strDirPath = Path.GetDirectoryName(strFilePath);
                if (Directory.Exists(strDirPath + "\\Stores"))
                {
                    Utils.AddStores2TaskScheduler(strDirPath + "\\Stores", strFilePath);
                }
                showHelp();
            }
            Utils.doSleep();
        }

        static void doFile(string strFile)
        {
            if (File.Exists(strFile))
            {
                Console.WriteLine("Processing: " + strFile);
                config conf = new config(strFile);
                if (conf.active)
                {
                    BC bigCommerce = new BC(conf.store_api, conf.store_user, conf.store_url, conf.store_lastid);
                    if (bigCommerce.newOrder)
                    {
                        SMS smsOut = new SMS(conf.sms_user, conf.sms_pass, conf.sms_url);
                        string strMessage = conf.store_name + " has received your order. Any questions please call " + conf.store_phone + ". Enjoy your food.";
                        long store_lastid = Convert.ToInt64(conf.store_lastid);
                        foreach (order dOrder in bigCommerce.orders)
                        {
                            Console.WriteLine(smsOut.send(dOrder.phone, strMessage));
                            if (dOrder.id > store_lastid) store_lastid = dOrder.id;
                        }
                        conf.store_lastid = (store_lastid + 1).ToString();
                    }
                }
            }
            else
            {
                Console.WriteLine("File not Found: " + strFile);
            }
        }

        static void showHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("      BCReader was sucessfully installed! ");
            Console.WriteLine("---------------------------------------------------- ");
            Console.WriteLine("  Now you should configure your stores (XML files)   ");
            Console.WriteLine("  and always confirm in the windows Task Scheduler   ");
            Console.WriteLine("---------------------------------------------------- ");
            Console.WriteLine("");
            Console.Write("Press ENTER to exit. ");
            Console.ReadLine();
        }
    }
}