using System;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Specialized;

namespace AWS_RiotAPI_Setter

{
   
 /*
    CurrentClient will hold static reference to the api key 
 */
    class CurrentClient
    {
        public static string apikey = ConfigurationManager.AppSettings.Get("apikey");
        
    }
 
    /*
        Class that adds to the database based off of which information is passed to TableAdd, pass in the MySqlConnection
        with the information for the items or the information for the runes. 
    */
       static class DatabaseConnect
     {
         // Adds item information to the database
         public static void TableAdd(String ChampName, List<int> champItems, List<String> champItemsNames, MySqlConnection connection)
         {
             
            String cmdText = "INSERT INTO ChampItems (ChampName, Item0, Item1, Item2,"
                + "Item3, Item4, Item5, ITEM6, ItemName0, ItemName1, ItemName2, ItemName3,"
                + "ItemName4, ItemName5, ItemName6) VALUES(@ChampName, @Item0, @Item1, @Item2,"
                + "@Item3, @Item4, @Item5, @Item6, @ItemName0, @ItemName1, @ItemName2, @ItemName3,"
                + "@ItemName4, @ItemName5, @ItemName6);";
     
       
            MySqlCommand cmd = new MySqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@ChampName", ChampName);
            cmd.Parameters.AddWithValue("@Item0", champItems[0]);
            cmd.Parameters.AddWithValue("@Item1", champItems[1]);
            cmd.Parameters.AddWithValue("@Item2", champItems[2]);
            cmd.Parameters.AddWithValue("@Item3", champItems[3]);
            cmd.Parameters.AddWithValue("@Item4", champItems[4]);
            cmd.Parameters.AddWithValue("@Item5", champItems[5]);
            cmd.Parameters.AddWithValue("@Item6", champItems[6]);
            cmd.Parameters.AddWithValue("@ItemName0", champItemsNames[0]);
            cmd.Parameters.AddWithValue("@ItemName1", champItemsNames[1]);
            cmd.Parameters.AddWithValue("@ItemName2", champItemsNames[2]);
            cmd.Parameters.AddWithValue("@ItemName3", champItemsNames[3]);
            cmd.Parameters.AddWithValue("@ItemName4", champItemsNames[4]);
            cmd.Parameters.AddWithValue("@ItemName5", champItemsNames[5]);
            cmd.Parameters.AddWithValue("@ItemName6", champItemsNames[6]);
            cmd.ExecuteNonQuery();
            
            }

            // Adds rune information to the database
          public static void TableAdd(String ChampName, List<String> champRunes, MySqlConnection connection){
            String cmdText = "INSERT INTO ChampRunes (ChampName, perkPrimaryStyle, perk0, perk1,"
                + "perk2, perk3, perkSubStyle, perk4, perk5) VALUES(@ChampName, @perkPrimaryStyle, @perk0, @perk1,"
                + "@perk2, @perk3, @perkSubStyle, @perk4, @perk5);";

            MySqlCommand cmd = new MySqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@ChampName", ChampName);
            cmd.Parameters.AddWithValue("@perkPrimaryStyle", champRunes[0]);
            cmd.Parameters.AddWithValue("@perk0", champRunes[1]);
            cmd.Parameters.AddWithValue("@perk1", champRunes[2]);
            cmd.Parameters.AddWithValue("@perk2", champRunes[3]);
            cmd.Parameters.AddWithValue("@perk3", champRunes[4]);
            cmd.Parameters.AddWithValue("@perkSubstyle", champRunes[5]);
            cmd.Parameters.AddWithValue("@perk4", champRunes[6]);
            cmd.Parameters.AddWithValue("@perk5", champRunes[7]);
            cmd.ExecuteNonQuery();
                       
          }  
          
         }

        // Configures the request for each "Getter" function and calls getter in getInfo to execute the request. 
        static class RequestCreator
        {
        public static JObject CreateRequest(String apiurl)
        {
        getInfo infoholder = new getInfo(apiurl);
        HttpClient client = new HttpClient();          
        return infoholder.getter(apiurl, client);
        }
        }



    class getInfo
    {   
        public string api_url;
        public getInfo(string url)
        {
            api_url = url;
        }

        // Executes the API call, looks for a 429 status code denoting rate limiting, checks the
        // response header for a retry-after value to sleep for that many second before executing any more
        // http requests. 
        public JObject getter(string url, HttpClient client)
        {
            String RetryAfterValue = "";
            client.BaseAddress = new Uri(api_url);
            // Add an Accept header for Json format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(CurrentClient.apikey).Result; // Will wait for response/timeout
            if ((int) response.StatusCode == 429)
            {
                Console.WriteLine("Rate Limited");
                var RetryAfterHeader = response.Headers.GetValues("Retry-After");
                foreach (var value in RetryAfterHeader){
                    RetryAfterValue = value;
                }
                Thread.Sleep(Int32.Parse(RetryAfterValue) * 1000);
                response = client.GetAsync(CurrentClient.apikey).Result;
            } 
          
            // Gets the API request
            var player_response = response.Content.ReadAsStringAsync().Result;
            // Converts to an object that correctly contains the API Call
            var result = JsonConvert.DeserializeObject(player_response); 
            // Convert the object to a Json object
            var token = JObject.FromObject(result);

            return token;
  
        }
    }
    /*
        Gets the names for the 200 challengers in the region
    */
       class ChallengerGetter
    {
        public JToken ChallengerName;
        public static List<string> ChallengerNames = new List<string>();
        public string apiurl = "https://na1.api.riotgames.com/lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5";

        private const int ChallengerCount = 200;

        public List<string> ChallNames()
        {

            JObject token = RequestCreator.CreateRequest(apiurl);
            ChallengerName = token["entries"];

            for (int i = 0; i < ChallengerCount; i++)
            {   // Adds each summoner name to a list
                ChallengerNames.Add(ChallengerName[i]["summonerName"].ToString());
               
            }
            return ChallengerNames;
        }

    }
    // Gets the required profile (account ID) for the summoner/challenger to get a list of matches played
    class SummonerGetter  
    {
        public string apiurl = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/";

        public string accID;
        
        public string GetAccID(string Name)
        {
            apiurl += Name;      // Appending the challenger name to url here
            JObject token = RequestCreator.CreateRequest(apiurl);
            accID = token.GetValue("accountId").ToString();
            return accID;
        }
    }
   
    // Takes the encryptedID parameter to get a list of matches for the challenger
    class MatchesGetter   // Parameter: SummonerGetter
                          // will grab the encrypted ID, to pass to getMatch
    {
        public static List<string> MatchList = new List<string>();
        public string apiurl = "https://na1.api.riotgames.com/lol/match/v4/matchlists/by-account/";

        public string GetEID(string sumAccID)
        {
            apiurl += sumAccID;
            JObject token = RequestCreator.CreateRequest(apiurl);
            for (int i = 0; i < token["endIndex"].ToObject<int>(); i++)

            {
                MatchList.Add(token["matches"][i]["gameId"].ToString());
            }
          
            return (token["matches"][0].ToString());
        }
    }
    // Gets match data for a specific match
    class MatchGetter // Parameter: encrypted ID    searches for a match with the requested champion.
                     
    {
        public string apiurl = "https://na1.api.riotgames.com/lol/match/v4/matches/";
        public JObject GetMatch(int mindex)
        {
            apiurl = apiurl + MatchesGetter.MatchList[mindex];
            JObject token = RequestCreator.CreateRequest(apiurl);
            return (token);
        }
    }
 
    class MatchDetails
    
    {
        public string[] summItems = new String[6];
        public string[] getItem(JObject token)
        {
            return summItems;
        }
    }

       class Program
    {
        const int CHAMPITEMNUMBER = 7;
        const int PLAYERCOUNT = 10;
        static void Main(string[] args)
        {
            
            // Creates a connection string using the credentials from the config file 
            MySqlConnection connection;
            String ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            connection = new MySqlConnection(ConnectionString);
            try{
                connection.Open();
                Console.WriteLine("Connection successfully opened");
            }
            catch (Exception){
                Console.WriteLine("Connection could not be established");
            }

            // urls for the Data Dragon jsons containing information related to champions, items, and runes. 
            string DCDragon = "http://ddragon.leagueoflegends.com/cdn/11.16.1/data/en_US/champion.json";
            string DIDragon = "http://ddragon.leagueoflegends.com/cdn/11.16.1/data/en_US/item.json";
            string DRDragon = "http://ddragon.leagueoflegends.com/cdn/11.16.1/data/en_US/runesReforged.json";
        
            // Iterator for the main loop
            int DatabaseIterator = 0;
            // Initalize lists to get the champion names and ids from data dragon
            List<String> ChampionNames = new List<string>();
            List<int> ChampionIDs = new List<int>();

            // Gets the Champion Data Dragon in a readable token 
            getInfo infoholderDC = new getInfo(DCDragon);
            HttpClient clientDC = new HttpClient(); 
            JObject DCtoken = infoholderDC.getter(DCDragon, clientDC);
           
           // Iterates through the seralized Champion token and puts each champion and ID in the corresponding lists.
            foreach (var ChampValues in DCtoken["data"].Values())
            {
                ChampionNames.Add(ChampValues["id"].ToString());
                ChampionIDs.Add((int)ChampValues["key"]);
            }
            // Iterate through each champion, returns when finished and closes the connection 
            BeginIteration(ChampionNames, DIDragon,ChampionIDs, DRDragon, connection, DatabaseIterator);
            connection.Close();
        }

        // Starts an iteration for each champion to find a game where the champion is used and retrieve the match data
    static void BeginIteration(List<String> ChampionNames, string DIDragon, List<int> ChampionIDs, String DRDragon, MySqlConnection connection, int DatabaseIterator){ 
        
        try{
            for (int k = DatabaseIterator; k < ChampionNames.Count; k++, DatabaseIterator++){
                // initalize lists for each champion and needed variables
                List<int> champItems = new List<int>();
                List<String> champItemsNames = new List<string>();
                List<JToken> ChampItemsIcon = new List<JToken>();
                List<string> matchChamps = new List<string>();
                Boolean foundchamp = false;
                Boolean inmatchlist = false;
                int requestedChamp = 0;
                String ChampName = "";
                int cindex = 0;     // Index of the challenger list being iterated
                int mindex = 0;     // Index of the match list being iterated.  
                int matchPID = 0;

                MatchesGetter.MatchList.Clear();       // Clears matchlist for each iteration    

                // Gets token for items from Data Dragon 
                getInfo infoholderDI = new getInfo(DIDragon);
                HttpClient clientDI = new HttpClient();
                JObject items = infoholderDI.getter(DIDragon, clientDI); 

                ChallengerGetter challengerG = new ChallengerGetter();
                List<string> Names = challengerG.ChallNames();

                SummonerGetter summonerG = new SummonerGetter();

                
        
                ChampName = ChampionNames[k];       // Gets the current champion we're looking for
                requestedChamp = ChampionIDs[k];    // Gets the id for that champion
                foundchamp= false;

                // Begins iterating through the matches for the requested champion 
                while (foundchamp == false)
                {
                    string sumAccID = summonerG.GetAccID(Names[cindex]);      
                    cindex++;
                    
                    MatchesGetter matchesG = new MatchesGetter();
                    string CurrentMatch = matchesG.GetEID(sumAccID);

                    // while the champion is not in the match and there are still matches to iterate through
                    while (inmatchlist == false && mindex < MatchesGetter.MatchList.Count)
                    {
                        MatchGetter matchG = new MatchGetter();
                        JObject match = matchG.GetMatch(mindex);
                        mindex++;
                        matchChamps.Clear();
                    
                        // Adds the champions from the match to matchCHamps
                        for (int i = 0; i < PLAYERCOUNT; i++)
                        {
                            matchChamps.Add(match["participants"][i]["championId"].ToString());
                        }
                        // Champion found
                        if (matchChamps.Contains(requestedChamp.ToString()))
                        {
                            int mlindex = matchChamps.IndexOf(requestedChamp.ToString());
                            
                            matchPID = mlindex;
                            foundchamp = true;
                            inmatchlist = true;
                        }
                        // Champion found loop
                        if (inmatchlist)
                        {
                            Console.WriteLine("Champion Items: \n");
                            for (int i = 0; i < CHAMPITEMNUMBER; i++)
                            {
                                // Gets the items for the champion
                                champItems.Add((int)match["participants"][matchPID]["stats"]["item" + i.ToString()]);
                                
                                if (champItems[i] != 0)
                                {
                                    Console.WriteLine(items["data"][(champItems[i]).ToString()]["name"]);
                                    champItemsNames.Add((items["data"][(champItems[i]).ToString()]["name"]).ToString());
                                    ChampItemsIcon.Add(items["data"][(champItems[i]).ToString()]["image"]);              //
                                }
                                // Puts a placeholder if an item slot is empty
                                if (champItems[i] == 0){
                                    champItemsNames.Add("Empty");
                                }
                                }

                    
                            // Gets the correct formatting for the runes 
                            ChampRunes champRunes = new ChampRunes(match, matchPID);

                            getInfo infoholderDR = new getInfo(DRDragon);
                            HttpClient clientDR = new HttpClient();
                            champRunes.GetRunesInfo(clientDR, DRDragon, connection, ChampName);
                        // Adds the gathered information to the database
                        DatabaseConnect.TableAdd(ChampName, champItems, champItemsNames, connection);
                        
                        }

                        }
                    }
                }
    }catch (System.NullReferenceException){
        Console.WriteLine("Exception (possible internet timeout), retrying");
        BeginIteration(ChampionNames, DIDragon,ChampionIDs, DRDragon, connection, DatabaseIterator);
}
            connection.Close();
            }
        }
    }




