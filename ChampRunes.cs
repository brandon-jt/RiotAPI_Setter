using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

/*
    The runes are stored in the matches as integers. They follow a different identification convention in the data dragon to get 
    the remaining rune information (see getRunesInfo). With this, we can properly automate accessing the rune page for data
    dragon and get the images and names we need. 
 */
class ChampRunes
    {
        int perk0id;  // Primary	
        int perk1id; // Primary
        int perk2id;    // Primary
        int perk3id; // Primary
        int perk4id; // Secondary
        int perk5id;    // Secondary
        int perkPrimaryStyleid; // Get this ("primary runes" family) Precision, Sorcery, Inspiration, etc
        int perkSubStyleid; // And this
        String perk0;
        String perk1;
        String perk2;
        String perk3;
        String perk4;
        String perk5;
        String perkPrimaryStyle;
        String perkSubStyle;
        String perk0icon;
        String perk1icon;
        String perk2icon;
        String perk3icon;
        String perk4icon;
        String perk5icon;
        String perkPrimaryStyleicon;
        String perkSubStyleicon;

        // Gets integer representation of runes
        public ChampRunes(JObject match, int matchPID)
        {
            perk0id = (int)match["participants"][matchPID]["stats"]["perk0"];
            perk1id = (int)match["participants"][matchPID]["stats"]["perk1"];
            perk2id = (int)match["participants"][matchPID]["stats"]["perk2"];
            perk3id = (int)match["participants"][matchPID]["stats"]["perk3"];
            perk4id = (int)match["participants"][matchPID]["stats"]["perk4"];
            perk5id = (int)match["participants"][matchPID]["stats"]["perk5"];
            perkPrimaryStyleid = (int)match["participants"][matchPID]["stats"]["perkPrimaryStyle"];
            perkSubStyleid = (int)match["participants"][matchPID]["stats"]["perkSubStyle"];

        }
        public void GetRunesInfo(HttpClient clientDR, string DRDragon, MySqlConnection connection, String ChampName)
        {
            Dictionary<int, int> RuneIDs = new Dictionary<int, int>();
            Dictionary<int, int> Slots = new Dictionary<int, int>();
            List<String> RunesList = new List<String>(); 
   
            RuneIDs[8100] = 0;  // Domination
            RuneIDs[8300] = 1;  // Inspiration
            RuneIDs[8000] = 2; // Precision
            RuneIDs[8400] = 3; // Resolve
            RuneIDs[8200] = 4; // Sorcery

            // Domination Runes 8100
            RuneIDs[8112] = 0; Slots[8112] = 0;
            RuneIDs[8124] = 1; Slots[8124] = 0;
            RuneIDs[8128] = 2; Slots[8128] = 0;
            RuneIDs[9923] = 3; Slots[9923] = 0;

            RuneIDs[8126] = 0; Slots[8126] = 1;
            RuneIDs[8139] = 1; Slots[8139] = 1;
            RuneIDs[8143] = 2; Slots[8143] = 1;

            RuneIDs[8136] = 0; Slots[8136] = 2;
            RuneIDs[8120] = 1; Slots[8120] = 2;
            RuneIDs[8138] = 2; Slots[8138] = 2;

            RuneIDs[8135] = 0; Slots[8135] = 3;
            RuneIDs[8134] = 1; Slots[8134] = 3;
            RuneIDs[8105] = 2; Slots[8105] = 3;
            RuneIDs[8106] = 3; Slots[8106] = 3;


            // Inspiration Runes 8300
            RuneIDs[8351] = 0; Slots[8351] = 0;
            RuneIDs[8360] = 1; Slots[8360] = 0;
            RuneIDs[8358] = 2; Slots[8358] = 0;

            RuneIDs[8306] = 0; Slots[8306] = 1;
            RuneIDs[8304] = 1; Slots[8304] = 1;
            RuneIDs[8313] = 2; Slots[8313] = 1;

            RuneIDs[8321] = 0; Slots[8321] = 2;
            RuneIDs[8316] = 1; Slots[8316] = 2;
            RuneIDs[8345] = 2; Slots[8345] = 2;

            RuneIDs[8347] = 0; Slots[8347] = 3;
            RuneIDs[8410] = 1; Slots[8410] = 3;
            RuneIDs[8352] = 2; Slots[8352] = 3;


            // Sorcery Runes [8200]
            RuneIDs[8214] = 0; Slots[8214] = 0;
            RuneIDs[8229] = 1; Slots[8229] = 0;
            RuneIDs[8230] = 2; Slots[8230] = 0;

            RuneIDs[8224] = 0; Slots[8224] = 1;
            RuneIDs[8226] = 1; Slots[8226] = 1;
            RuneIDs[8275] = 2; Slots[8275] = 1;

            RuneIDs[8210] = 0; Slots[8210] = 2;
            RuneIDs[8234] = 1; Slots[8234] = 2;
            RuneIDs[8233] = 2; Slots[8233] = 2;

            RuneIDs[8237] = 0; Slots[8237] = 3;
            RuneIDs[8232] = 1; Slots[8232] = 3;
            RuneIDs[8236] = 2; Slots[8236] = 3;


            // Precision Runes [8000]
            RuneIDs[8005] = 0; Slots[8005] = 0;
            RuneIDs[8008] = 1; Slots[8008] = 0;
            RuneIDs[8021] = 2; Slots[8021] = 0;
            RuneIDs[8010] = 3; Slots[8010] = 0;

            RuneIDs[9101] = 0; Slots[9101] = 1;
            RuneIDs[9111] = 1; Slots[9111] = 1;
            RuneIDs[8009] = 2; Slots[8009] = 1;

            RuneIDs[9104] = 0; Slots[9104] = 2;
            RuneIDs[9105] = 1; Slots[9105] = 2;
            RuneIDs[9103] = 2; Slots[9103] = 2;

            RuneIDs[8014] = 0; Slots[8014] = 3;
            RuneIDs[8017] = 1; Slots[8017] = 3;
            RuneIDs[8299] = 2; Slots[8299] = 3;



            // Resolve Runes [8400]
            RuneIDs[8437] = 0; Slots[8437] = 0;
            RuneIDs[8439] = 1; Slots[8439] = 0;
            RuneIDs[8465] = 2; Slots[8465] = 0;

            RuneIDs[8446] = 0; Slots[8446] = 1;
            RuneIDs[8463] = 1; Slots[8463] = 1;
            RuneIDs[8401] = 2; Slots[8401] = 1;

            RuneIDs[8429] = 0; Slots[8429] = 2;
            RuneIDs[8444] = 1; Slots[8444] = 2;
            RuneIDs[8473] = 2; Slots[8473] = 2;

            RuneIDs[8451] = 0; Slots[8451] = 3;
            RuneIDs[8453] = 1; Slots[8453] = 3;
            RuneIDs[8242] = 2; Slots[8242] = 3;

            HttpResponseMessage runesResponse = clientDR.GetAsync(DRDragon).Result;
            var RunesString = runesResponse.Content.ReadAsStringAsync().Result;
            
            var RunesObject = JArray.Parse(RunesString);           

            //Console.WriteLine(RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[8112]]["runes"][RuneIDs[8112]]);


            perkPrimaryStyle = RunesObject[RuneIDs[perkPrimaryStyleid]]["name"].ToString();
            perkPrimaryStyleicon = RunesObject[RuneIDs[perkPrimaryStyleid]]["icon"].ToString();
            RunesList.Add(perkPrimaryStyle);

            perk0 = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk0id]]["runes"][RuneIDs[perk0id]]["name"].ToString();
            perk0icon = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk0id]]["runes"][RuneIDs[perk0id]]["icon"].ToString();
            RunesList.Add(perk0);

            perk1 = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk1id]]["runes"][RuneIDs[perk1id]]["name"].ToString();
            perk1icon = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk1id]]["runes"][RuneIDs[perk1id]]["icon"].ToString();
            RunesList.Add(perk1);

            perk2 = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk2id]]["runes"][RuneIDs[perk2id]]["name"].ToString();
            perk2icon = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk2id]]["runes"][RuneIDs[perk2id]]["icon"].ToString();
            RunesList.Add(perk2);

            perk3 = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk3id]]["runes"][RuneIDs[perk3id]]["name"].ToString();
            perk3icon = RunesObject[RuneIDs[perkPrimaryStyleid]]["slots"][Slots[perk3id]]["runes"][RuneIDs[perk3id]]["icon"].ToString();
            RunesList.Add(perk3);

            perkSubStyle = RunesObject[RuneIDs[perkSubStyleid]]["name"].ToString();
            perkSubStyleicon = RunesObject[RuneIDs[perkSubStyleid]]["icon"].ToString();
            RunesList.Add(perkSubStyle);

            perk4 = RunesObject[RuneIDs[perkSubStyleid]]["slots"][Slots[perk4id]]["runes"][RuneIDs[perk4id]]["name"].ToString();
            perk4icon = RunesObject[RuneIDs[perkSubStyleid]]["slots"][Slots[perk4id]]["runes"][RuneIDs[perk4id]]["icon"].ToString();
            RunesList.Add(perk4);

            perk5 = RunesObject[RuneIDs[perkSubStyleid]]["slots"][Slots[perk5id]]["runes"][RuneIDs[perk5id]]["name"].ToString();
            perk5icon = RunesObject[RuneIDs[perkSubStyleid]]["slots"][Slots[perk5id]]["runes"][RuneIDs[perk5id]]["icon"].ToString();
            RunesList.Add(perk5);

            Console.WriteLine("Primary:" + " " + perkPrimaryStyle);
            Console.WriteLine("Runes: " + perk0 + " " +  perk1 + " " + perk2 + " " + perk3);
            Console.WriteLine("Secondary:" + " " +  perkSubStyle);
            Console.WriteLine("Runes:" + " " + perk4 + " " + perk5);
            
            AWS_RiotAPI_Setter.DatabaseConnect.TableAdd(ChampName, RunesList, connection);
    
             String cmdText = "INSERT INTO ChampRunes (ChampName, perkPrimaryStyle, perk0, perk1,"
                            + "perk2, perk3, perkSubStyle, perk4, perk5) VALUES(@ChampName, @perkPrimaryStyle, @perk0, @perk1,"
                            + "@perk2, @perk3, @perkSubStyle, @perk4, @perk5);";
                        MySqlCommand cmd = new MySqlCommand(cmdText, connection);
                        cmd.Parameters.AddWithValue("@ChampName", ChampName);
                        cmd.Parameters.AddWithValue("@perkPrimaryStyle", perkPrimaryStyle);
                        cmd.Parameters.AddWithValue("@perk0", perk0);
                        cmd.Parameters.AddWithValue("@perk1", perk1);
                        cmd.Parameters.AddWithValue("@perk2", perk2);
                        cmd.Parameters.AddWithValue("@perk3", perk3);
                        cmd.Parameters.AddWithValue("@perkSubstyle", perkSubStyle);
                        cmd.Parameters.AddWithValue("@perk4", perk4);
                        cmd.Parameters.AddWithValue("@perk5", perk5);
                        cmd.ExecuteNonQuery();

          
        }
    }