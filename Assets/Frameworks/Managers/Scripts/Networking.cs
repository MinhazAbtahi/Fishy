using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

namespace FPG
{
    public class Networking
    {

#if UNITY_IOS || UNITY_IPHONE
        static string serverFolderName = "dinowaterworld3dios";
#elif UNITY_ANDROID
        //static string serverFolderName = "dinowaterworld3dgp";
        static string serverFolderName = "testunity";
#endif
#if UNITY_IOS || UNITY_IPHONE ||  UNITY_ANDROID
        static string serverLink = "https://gamedatadb.herokuapp.com/" + serverFolderName + "/";
#endif
        static Networking sharedInstance;

        public int total_ad_show_counter = 0;
        public int total_int_ad_show_counter = 0;
        public int total_cross_promo_ad_show_counter = 0;
        public string cross_promo_ad_bundle_id;
        public string cross_promo_ad_category;
        public string cross_promo_ad_type;
        public string cross_promo_ad_source = "0";
        public string video_ad_source = "0";

        public int videoAdImpression = 0;

        public int selectedAd = 0;
        public static Networking getInstance()
        {
            if (sharedInstance == null)
            {
                sharedInstance = Networking.create();
            }

            return sharedInstance;
        }

        static Networking create()
        {
            Networking ret = new Networking();
            if (ret != null && ret.init())
            {
                return ret;
            }
            else
            {
                //ret.Dispose();
                ret = null;
                return null;
            }
        }
        public bool init()
        {
            return true;
        }

        public void SetGameStatus()
        {

            // setPlayerSession
            int numberOfSessionPlayed = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);
            bool firstTime = false;
            if (numberOfSessionPlayed == 0)
            {
                firstTime = true;
                string date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                PlayerPrefs.SetString("fristTimePlayedDateStr", date);
            }

            numberOfSessionPlayed++;
            PlayerPrefs.SetInt("numberOfSessionPlayed", numberOfSessionPlayed);
            PlayerPrefs.SetString("sessionStartTimeStamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

            if (firstTime)
                sendUserActivity("WelcomeOkTap");
            else
                sendUserActivity("GameStart");


            sendUnPaidUserCurrentStatus();

        }

        public void sendUnPaidUserCurrentStatus()
        {
            string tableName = "send_unpaid_user_current_status";

            string fristTimePlayedDateStr = PlayerPrefs.GetString("fristTimePlayedDateStr", "0");
            string current_timeDateStr = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            int numberOfSessionPlayed = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);
            string lastPurchaseTimeDateStr = "0";
            string lastInappIdentifier = "0";
            string lastInAppPrice = "0";
            //DBUserInfo *userInfo = DBUserInfo::create();
            int userLevel = PlayerPrefs.GetInt("TotalGameCount", 1);
            int userCoins = PlayerPrefs.GetInt(GameManager.totalcoinKey, 0);
            int userFoods = PlayerPrefs.GetInt(GameManager.totalWoodKey, 0);
            int userBucks = 0;
            int lifeAmount = 0;
            string countryCode = "0";
            string language = Application.systemLanguage.ToString();
            string currency = "0";
            string fristTimePlayedDateStrManual = "0";
            string createdFusionData = "0";

            int totalLeagueCompleted = 0;

            int dailyBonusCollectDayNumber = 0;
            string deviceModel = SystemInfo.deviceModel;
            string deviceOs = SystemInfo.operatingSystem;
            string currentAppVersion = Application.version;
            int promoStone = 0;

            int userDomainId = 0;// (int)AppDelegate.GetDomainType();

            string allDataString = "Play_Start_Date=" + fristTimePlayedDateStr;
            allDataString = allDataString + "&Last_Played=" + current_timeDateStr;
            allDataString = allDataString + "&Total_Sessions=" + numberOfSessionPlayed.ToString();
            allDataString = allDataString + "&Last_Purchase_Time=" + lastPurchaseTimeDateStr;
            allDataString = allDataString + "&Last_Purchase_Item=" + lastInappIdentifier;
            allDataString = allDataString + "&Last_Purchase_Price=" + lastInAppPrice;
            allDataString = allDataString + "&User_Level=" + userLevel.ToString();
            allDataString = allDataString + "&User_Coins=" + userCoins.ToString();
            allDataString = allDataString + "&User_Foods=" + userFoods.ToString();
            allDataString = allDataString + "&User_Bucks=" + userBucks.ToString();
            allDataString = allDataString + "&User_Energy=" + lifeAmount.ToString();
            allDataString = allDataString + "&Country=" + countryCode;
            allDataString = allDataString + "&Language=" + language;
            allDataString = allDataString + "&Currency=" + currency;
            allDataString = allDataString + "&c1=" + fristTimePlayedDateStrManual;
            allDataString = allDataString + "&c2=" + createdFusionData;
            allDataString = allDataString + "&c3=" + totalLeagueCompleted.ToString();
            //    allDataString = allDataString+"&c4="+to_string(arenaid);
            allDataString = allDataString + "&c4=" + dailyBonusCollectDayNumber.ToString();
            allDataString = allDataString + "&c5=" + deviceModel;
            allDataString = allDataString + "&c6=" + deviceOs;
            allDataString = allDataString + "&c7=" + currentAppVersion;
            allDataString = allDataString + "&c8=" + promoStone.ToString();
            allDataString = allDataString + "&c9=" + SystemInfo.processorType.ToString();
            allDataString = allDataString + "&domain_id=" + userDomainId.ToString();

            string udid = SystemInfo.deviceUniqueIdentifier;

            sendDataToServer(tableName, udid, allDataString);
            //Log.L("data send");
        }

        public void send_unpaid_user_battle_status(int p_attempt, int p_win, int p_loose, GameData type = GameData.LevelCount)
        {

            string tableName = "send_unpaid_user_battle_status";

            string sessionStartTimeStampStr = PlayerPrefs.GetInt("TotalGameCount", 1).ToString();
            int _battleId = GameManager.GetGameData(type); ;// (MapManager.SharedManager().currentMapIndex * Constants.MaxPlayCount) + PlayerPrefs.GetInt(Constants.GetMapIndexKey(MapManager.SharedManager().currentMapIndex), 0);
                                                     //int _battleId = PlayerPrefs.GetInt(Constants.totalVictory,1);

            int battle_id = _battleId;

            int attempt = p_attempt;

            int loose = p_loose;

            int win = p_win;

            int numberOfSessionPlayed = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);

            int userBucks = 0;

            int userCoins = PlayerPrefs.GetInt(GameManager.totalcoinKey, 0);
            int userFoods = PlayerPrefs.GetInt(GameManager.totalWoodKey, 0);

            int lifeAmount = 0;


            int userLevel = PlayerPrefs.GetInt("TotalGameCount", 1);

            string c1 = Application.version;

            string c2 = "";

            string c3 = "";

            string fristTimePlayedDateStr = PlayerPrefs.GetString("fristTimePlayedDateStr", "0");

            string c5 = "";

            string c6 = "";

            string c7 = "";

            string c8 = GameManager.botType.ToString();

            string c9 = SystemInfo.processorType;

            int userDomainId = 0;// (int)AppDelegate.GetDomainType();

            string allDataString = "Battle_Id=" + battle_id.ToString();
            // allDataString = allDataString + "ostype=" + ostype.ToString();
            // allDataString = allDataString + "paidstatus=" + paidstatus.ToString();
            allDataString = allDataString + "&Attempt=" + attempt.ToString();
            allDataString = allDataString + "&Loose=" + loose.ToString();
            allDataString = allDataString + "&Win=" + win.ToString();
            allDataString = allDataString + "&Total_Sessions=" + numberOfSessionPlayed.ToString();
            allDataString = allDataString + "&User_Bucks=" + userBucks.ToString();
            allDataString = allDataString + "&User_Coins=" + userCoins.ToString();
            allDataString = allDataString + "&User_Energy=" + lifeAmount.ToString();
            allDataString = allDataString + "&User_Foods=" + userFoods.ToString();
            allDataString = allDataString + "&User_Level=" + userLevel.ToString();
            allDataString = allDataString + "&c1=" + c1.ToString();
            allDataString = allDataString + "&c2=" + c2.ToString();
            allDataString = allDataString + "&c3=" + c3.ToString();
            allDataString = allDataString + "&c4=" + fristTimePlayedDateStr;
            allDataString = allDataString + "&c5=" + c5.ToString();
            allDataString = allDataString + "&c6=" + c6.ToString();
            allDataString = allDataString + "&c7=" + c7.ToString();
            allDataString = allDataString + "&c8=" + c8.ToString();
            allDataString = allDataString + "&c9=" + c9.ToString();
            allDataString = allDataString + "&Domain_Id=" + userDomainId.ToString();
            string udid = SystemInfo.deviceUniqueIdentifier;

            sendDataToServer(tableName, udid, allDataString);
        }

        public void sendUserActivity(string activityName)
        {

            string tableName = "send_unpaid_user_session_activity";

            int numberOfSessionPlayed = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);
            string sessionStartTimeStampStr = PlayerPrefs.GetString("sessionStartTimeStamp", "0");
            string current_timeDateStr = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            if (numberOfSessionPlayed <= 5)
            {
                int userBucks = 0;

                int userCoins = PlayerPrefs.GetInt(GameManager.totalcoinKey, 0);
                int userFoods = PlayerPrefs.GetInt(GameManager.totalWoodKey, 0);

                int userLevel = PlayerPrefs.GetInt("TotalGameCount", 1);

                string c1 = Application.version;

                string c2 = Networking.getInstance().videoAdImpression.ToString();

                int userDomainId = 0;// (int)AppDelegate.GetDomainType();

                string allDataString = "numberOfSessionPlayed=" + numberOfSessionPlayed.ToString();
                allDataString = allDataString + "&data=Session_Start_Time-tappocket-" + sessionStartTimeStampStr;
                allDataString = allDataString + "-itiw-Last_Played-tappocket-" + current_timeDateStr;
                allDataString = allDataString + "-itiw-" + activityName + "-tappocket-" + (Int32.Parse(current_timeDateStr) - Int32.Parse(sessionStartTimeStampStr) + 1).ToString();
                allDataString = allDataString + "-itiw-userLevel-tappocket-" + userLevel.ToString();
                allDataString = allDataString + "-itiw-User_Coins-tappocket-" + userCoins.ToString();
                allDataString = allDataString + "-itiw-User_Foods-tappocket-" + userFoods.ToString();
                allDataString = allDataString + "-itiw-User_Bucks-tappocket-" + userBucks.ToString();
                allDataString = allDataString + "-itiw-User_DomainId-tappocket-" + userDomainId.ToString();
                allDataString = allDataString + "-itiw-currentAppVersion-tappocket-" + c1;
                allDataString = allDataString + "-itiw-AdsImpression-tappocket-" + c2;

                string udid = SystemInfo.deviceUniqueIdentifier;

                sendDataToServer(tableName, udid, allDataString);
                //Log.L("data send ------------------------->>>>>"+ (Convert.ToInt64(current_timeDateStr) - Convert.ToInt64(sessionStartTimeStampStr) + 1).ToString());
            }

        }

        public void sendUserCrossPromoAdStatus(int ad_start, int ad_closed, int adshow_complete, int install_clicked, int store_open, int game_open, string source)
        {
            long sessionStartTime = (long)Convert.ToDouble(PlayerPrefs.GetString("sessionStartTimeStamp", "0"));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long time_stamp = currentTime - sessionStartTime;
            string selected_timestamp = "ad_start_timestamp";
            string selected_srouce = "0";

            if (ad_start == 1)
            {
                selected_timestamp = "ad_start_timestamp";
                selected_srouce = "ad_start_source";
            }
            else if (ad_closed == 1)
            {
                selected_timestamp = "ad_closed_timestamp";
            }
            else if (adshow_complete == 1)
            {
                selected_timestamp = "adshow_complete_timestamp";
                selected_srouce = "adshow_complete_source";
            }
            else if (install_clicked == 1)
            {
                selected_timestamp = "install_clicked_timestamp";
            }
            else
            {
                selected_timestamp = "ad_start_timestamp";
            }

            int cross_promo_adid = PlayerPrefs.GetInt("CrossPromoAdCounter", 1);
            int total_session = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);
            int total_ad_show = total_cross_promo_ad_show_counter;
            string country = "0";
            string language = Application.systemLanguage.ToString();
            string os_version = SystemInfo.operatingSystem;
            string currentAppVersion = Application.version;


            string fristTimePlayedDateStr = PlayerPrefs.GetString("fristTimePlayedDateStr", "0");

            int user_bucks = 0;
            int user_coins = PlayerPrefs.GetInt(GameManager.totalcoinKey, 0);
            int user_foods = PlayerPrefs.GetInt(GameManager.totalWoodKey, 0);
            int user_level = PlayerPrefs.GetInt("TotalGameCount", 1);

            string allDataString = "adid=" + cross_promo_adid.ToString();
            allDataString = allDataString + "&total_session=" + total_session.ToString();
            allDataString = allDataString + "&total_ad_show=" + total_ad_show.ToString();
            allDataString = allDataString + "&ad_start=" + ad_start.ToString();
            allDataString = allDataString + "&ad_closed=" + ad_closed.ToString();
            allDataString = allDataString + "&adshow_complete=" + adshow_complete.ToString();
            allDataString = allDataString + "&install_clicked=" + install_clicked.ToString();
            allDataString = allDataString + "&store_open=" + store_open.ToString();
            allDataString = allDataString + "&game_open=" + game_open.ToString();
            allDataString = allDataString + "&country=" + country;
            allDataString = allDataString + "&Language=" + language;
            allDataString = allDataString + "&" + selected_srouce + "=" + source;
            allDataString = allDataString + "&os_version=" + os_version;
            allDataString = allDataString + "&appversion=" + currentAppVersion;
            allDataString = allDataString + "&user_bucks=" + user_bucks.ToString();
            allDataString = allDataString + "&user_coins=" + user_coins.ToString();
            allDataString = allDataString + "&user_foods=" + user_foods.ToString();
            allDataString = allDataString + "&user_level=" + user_level.ToString();
            allDataString = allDataString + "&" + selected_timestamp + "=" + time_stamp.ToString();
            allDataString = allDataString + "&c1=" + cross_promo_ad_bundle_id;
            allDataString = allDataString + "&c2=" + cross_promo_ad_category;
            allDataString = allDataString + "&c3=" + cross_promo_ad_type;

            string udid = SystemInfo.deviceUniqueIdentifier;
            string tableName = "cross_promo_ad_status";

            sendDataToServer(tableName, udid, allDataString);
        }

        public void sendUserAdStatus(int request_sent, int adload_complete, int adload_failed, int ad_show, int ad_cross, int ad_click, int adshow_complete, string source, bool isRewardedAd = true)
        {
            long sessionStartTime = (long)Convert.ToDouble(PlayerPrefs.GetString("sessionStartTimeStamp", "0"));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long time_stamp = currentTime - sessionStartTime;
            string selected_timestamp = "request_sent_timestamp";
            string selected_srouce = "0";

            if (request_sent == 1)
            {
                selected_timestamp = "request_sent_timestamp";
                selected_srouce = "adrequest_source";
            }
            else if (adload_complete == 1)
            {
                selected_timestamp = "adload_complete_timestamp";
            }
            else if (adload_failed == 1)
            {
                selected_timestamp = "adload_complete_timestamp";
            }
            else if (ad_show == 1)
            {
                selected_timestamp = "ad_show_timestamp";
                selected_srouce = "adshow_source";
            }
            else if (ad_cross == 1)
            {
                selected_timestamp = "ad_cross_timestamp";
            }
            else if (ad_click == 1)
            {
                selected_timestamp = "ad_click_timestamp";

            }
            else if (adshow_complete == 1)
            {
                selected_timestamp = "adshow_complete_timestamp";
            }
            else
            {
                selected_timestamp = "request_sent_timestamp";
            }

            if (ad_show == 1 && isRewardedAd == false)
            {
                selected_timestamp = "ad_show_timestamp";
                selected_srouce = "adshow_source";
            }

            int adid = PlayerPrefs.GetInt("AdCounter", 0);
            int total_session = PlayerPrefs.GetInt("numberOfSessionPlayed", 0);
            int total_ad_show = total_ad_show_counter;
            string country = "0";
            string os_version = SystemInfo.operatingSystem;

            //CCLOG("total_ad_show_counter: %d total_ad_show: %d", total_ad_show_counter, total_ad_show);

            string fristTimePlayedDateStr = PlayerPrefs.GetString("fristTimePlayedDateStr", "0");//"0";

            int user_bucks = 0;
            int user_coins = PlayerPrefs.GetInt(GameManager.totalcoinKey, 0);
            int user_foods = PlayerPrefs.GetInt(GameManager.totalWoodKey, 0);
            int user_level = 1; // GameManager.getBattleId((int)GameName.CacthTheTheif);
            string currentAppVersion = Application.version;
            string adType = "rw";

            if (isRewardedAd == false)
            {
                adid = PlayerPrefs.GetInt("IntAdCounter", 100000);
                adType = "int";
                total_ad_show = total_int_ad_show_counter;
            }

            int userDomainId = 0;// (int)AppDelegate.GetDomainType();

            string allDataString = "adid=" + adid;
            allDataString = allDataString + "&total_session=" + total_session;
            allDataString = allDataString + "&total_ad_show=" + total_ad_show;
            allDataString = allDataString + "&request_sent=" + request_sent;
            allDataString = allDataString + "&adload_complete=" + adload_complete;
            allDataString = allDataString + "&adload_failed=" + adload_failed;
            allDataString = allDataString + "&ad_show=" + ad_show;
            allDataString = allDataString + "&ad_cross=" + ad_cross;
            allDataString = allDataString + "&ad_click=" + ad_click;
            allDataString = allDataString + "&adshow_complete=" + adshow_complete;
            allDataString = allDataString + "&country=" + country;
            allDataString = allDataString + "&" + selected_srouce + "=" + source;
            allDataString = allDataString + "&os_version=" + currentAppVersion;
            //allDataString = allDataString + "&appversion=" + currentAppVersion;
            allDataString = allDataString + "&user_bucks=" + user_bucks.ToString();
            allDataString = allDataString + "&user_coins=" + user_coins.ToString();
            allDataString = allDataString + "&user_foods=" + user_foods.ToString();
            allDataString = allDataString + "&user_level=" + user_level.ToString();
            allDataString = allDataString + "&" + selected_timestamp + "=" + time_stamp.ToString();
            allDataString = allDataString + "&c1=" + selectedAd.ToString();
            allDataString = allDataString + "&c2=" + userDomainId.ToString();
            allDataString = allDataString + "&c3=" + adType;

            string udid = SystemInfo.deviceUniqueIdentifier;
            string tableName = "rewarded_ad_status";

            sendDataToServer(tableName, udid, allDataString);
            //this.sendDataToServer(udid, allDataString);
        }
        private static void GetLoginResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                //Stream streamResponse = response.GetResponseStream();
                //StreamReader streamRead = new StreamReader(streamResponse);
                //string responseString = streamRead.ReadToEnd();

                //Log.L("response type --> " + responseString);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Log.L("Link hit is successfull");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    //Log.L("NotFound");

                }
                else
                {
                    //Log.L("404");
                }
                // Close the stream object
                //streamResponse.Close();
                //streamRead.Close();
                response.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine("\n Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }

        }

        public void sendDataToServer(string tableName, string udid, string allDataString)
        {

#if UNITY_EDITOR
            return;
#endif
            allDataString.Replace(" ", "");
            if (udid != "0")
            {
                string url = serverLink + tableName + "?udid=" + udid + "&" + allDataString;
                Debug.Log("data send--->" + url);
                try
                {
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    //optional
                    /*HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    //Stream stream = response.GetResponseStream();
                    //StreamReader readStream = new StreamReader(stream, Encoding.UTF8);


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Log.L("Link hit is success");
                    }
                    else
                    {
                        Log.L("Link hit is unsuccessful");
                    }

                    response.Close();
                    //readStream.Close();*/

                    request.BeginGetResponse(new AsyncCallback(GetLoginResponseCallback), request);
                }
                catch (WebException ex)
                {
                    //Log.L(ex.ToString());
                    HttpStatusCode wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    if (wRespStatusCode == HttpStatusCode.NotFound)
                    {
                        //Log.L("NotFound");
                    }
                    else
                    {
                        //Log.L("404");
                    }
                }
            }
        }
    }
}