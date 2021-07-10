using System;
namespace FPG
{
    public class AdPlacement
    {
        private AdPlacement() { }
#if UNITY_ANDROID || UNITY_STANDALONE_LINUX

        //Dino Water World 3D
        public static readonly string admob_app_id = "ca-app-pub-9899159862856473~3570875236";


        public static readonly string admob_adunit_id_25 = "ca-app-pub-9899159862856473/2647486435";
        public static readonly string admob_adunit_id_20 = "ca-app-pub-9899159862856473/3624707949";
        public static readonly string admob_adunit_id_15 = "ca-app-pub-9899159862856473/7372381266";
        public static readonly string admob_adunit_id_10 = "ca-app-pub-9899159862856473/9069424554";
        public static readonly string admob_adunit_id_07 = "ca-app-pub-9899159862856473/2504016200";
        public static readonly string admob_adunit_id_05 = "ca-app-pub-9899159862856473/4034103189";
        public static readonly string admob_adunit_id_03 = "ca-app-pub-9899159862856473/8829751404";
        public static readonly string admob_adunit_id_default = "ca-app-pub-9899159862856473/3577424727";

        public static readonly string fb_placement_id = "2667526753468737_2669985549889524";
        public static readonly string fb_placement_id_high = "2667526753468737_2669986306556115";
        public static readonly string fb_placement_id_medium = "2667526753468737_2669986536556092";
        public static readonly string fb_placement_id_low = "2667526753468737_2669986933222719";
        public static readonly string fb_placement_id_default = "2667526753468737_2669987849889294";
        public static readonly string fb_placement_id_upperTH = "2667526753468737_2669987849889294";
        public static readonly string fb_placement_id_lowerTH = "2667526753468737_2669987849889294";

        public static readonly string facebook_interstitial_id_01 = "2667526753468737_2669985549889524";
        public static readonly string facebook_interstitial_id_02 = "2667526753468737_2669985926556153";

        public static readonly string admob_interstitial_id_01 = "ca-app-pub-9899159862856473/8138668029";
        public static readonly string admob_interstitial_id_02 = "ca-app-pub-9899159862856473/8139486266";
        public static readonly string admob_interstitial_id_03 = "ca-app-pub-9899159862856473/4007851329";
        public static readonly string admob_interstitial_id_04 = "ca-app-pub-9899159862856473/1381687987";


        public const string ironsrc_app_key = "abd202fd"; //Shark Attack ios
        public const string ironsrc_default_interstitialKey = "DefaultInterstitial"; //Shark Attack ios
        public const string ironsrc_default_rewardedKey = "DefaultRewardedVideo"; //Shark Attack ios

        public const string applovin_app_key = "OTMEVTsH7Scq3AMcx7FTcN_EU2UGpFJik6TRCdNPV9T0c_fQvQzETGaEZTaG4U7ugJG0r1ZmT2eLNR4SwTqQCQ";//dww3d ios
        public const string applovin_default_interstitial_key = "630e7bc9c0f7a446";//sa3d
        public const string applovin_default_rewarded_key = "778dcdf472545f4c";//sa3d
        public const string applovin_default_banner_key = "cefd623ff0374291";//sa3d

#elif UNITY_IOS
//Dino Water World 3D iOS
        public static readonly string admob_app_id = "ca-app-pub-9899159862856473~1639465011";


        public static readonly string admob_adunit_id_25 = "ca-app-pub-9899159862856473/2944807828";
        public static readonly string admob_adunit_id_20 = "ca-app-pub-9899159862856473/6309337764";
        public static readonly string admob_adunit_id_15 = "ca-app-pub-9899159862856473/5543051002";
        public static readonly string admob_adunit_id_10 = "ca-app-pub-9899159862856473/1412234300";
        public static readonly string admob_adunit_id_07 = "ca-app-pub-9899159862856473/2286332274";
        public static readonly string admob_adunit_id_05 = "ca-app-pub-9899159862856473/9781678919";
        public static readonly string admob_adunit_id_03 = "ca-app-pub-9899159862856473/8277025559";
        public static readonly string admob_adunit_id_default = "ca-app-pub-9899159862856473/9398535533";

        public static readonly string fb_placement_id = "502707693775628_502712630441801";
        public static readonly string fb_placement_id_high = "502707693775628_502711497108581";
        public static readonly string fb_placement_id_medium = "502707693775628_502711713775226";
        public static readonly string fb_placement_id_low = "502707693775628_502712010441863";
        public static readonly string fb_placement_id_default = "502707693775628_502716497108081";
        public static readonly string fb_placement_id_upperTH = "502707693775628_502716497108081";
        public static readonly string fb_placement_id_lowerTH = "502707693775628_502716497108081";

        public static readonly string facebook_interstitial_id_01 = "502707693775628_502712630441801";
        public static readonly string facebook_interstitial_id_02 = "502707693775628_502713243775073";

        public static readonly string admob_interstitial_id_01 = "ca-app-pub-9899159862856473/7682145097";
        public static readonly string admob_interstitial_id_02 = "ca-app-pub-9899159862856473/6528829612";
        public static readonly string admob_interstitial_id_03 = "ca-app-pub-9899159862856473/2398012912";
        public static readonly string admob_interstitial_id_04 = "ca-app-pub-9899159862856473/4641032876";

        
        public const string ironsrc_app_key = "abd202fd"; //Shark Attack ios
        public const string ironsrc_default_interstitialKey = "DefaultInterstitial"; //Shark Attack ios
        public const string ironsrc_default_rewardedKey = "DefaultRewardedVideo"; //Shark Attack ios
        
        public const string applovin_app_key = "OTMEVTsH7Scq3AMcx7FTcN_EU2UGpFJik6TRCdNPV9T0c_fQvQzETGaEZTaG4U7ugJG0r1ZmT2eLNR4SwTqQCQ";//dww3d ios
        public const string applovin_default_interstitial_key = "5dcc5f4b87d554b6";//dww3d ios
        public const string applovin_default_rewarded_key = "ee2c29f3120dc288";//dww3d ios
        public const string applovin_default_banner_key = "";//dww3d ios

        //SkAdNetwork

        public const string Applovin_SKADID = "ludvb6z3bs.skadnetwork";
        public const string Ironsrc_SKADID = "su67r6k2v3.skadnetwork";
        public const string Admob_SKADID = "cstr6suwn9.skadnetwork";
        public const string UnityAds_SKADID = "4dzt52r2t5.skadnetwork";
        public const string Adcolony_SKADID = "4pfyvq9l8r.skadnetwork";
        public const string Facebook1_SKADID = "v9wttpbfk9.skadnetwork";
        public const string Facebook2_SKADID = "n38lu8286q.skadnetwork";
        public const string Mintegral_SKADID = "KBD757YWX3.skadnetwork";
        public const string Tapjoy_SKADID = "ecpz2srf59.skadnetwork";
        public const string Pangle_NonCN_SKADID = "22mmun2rn5.skadnetwork";
        public const string Pangle_CN_SKADID = "238da6jt44.skadnetwork";
        public const string Vungle_SKADID = "gta9lk7p23.skadnetwork";

        public static string[] SkAdIds = { Applovin_SKADID, Ironsrc_SKADID, Admob_SKADID, UnityAds_SKADID, Adcolony_SKADID, Facebook1_SKADID,
        Facebook2_SKADID, Mintegral_SKADID ,Tapjoy_SKADID, Pangle_NonCN_SKADID, Pangle_CN_SKADID, Vungle_SKADID};
#endif
    }
}