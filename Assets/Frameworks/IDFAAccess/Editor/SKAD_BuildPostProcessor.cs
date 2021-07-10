// filename SKAD_BuildPostProcessor.cs
// put it in a folder Assets/Editor/
#define IOS14
#if UNITY_IOS
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class SKAD_BuildPostProcessor
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    {
        Debug.Log("ashlog: ChangeXcodePlist");

        if (buildTarget == BuildTarget.iOS)
        {

            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            Debug.Log(">> Automation, plist ... <<");

            // example of changing a value:
            // rootDict.SetString("CFBundleVersion", "6.6.6");

            // example of adding a boolean key...
            // < key > ITSAppUsesNonExemptEncryption </ key > < false />
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

#if IOS14
            IOS14_AddPlistEntries(rootDict);
#endif

            File.WriteAllText(plistPath, plist.WriteToString());

            //EditUnityAppController(path);
        }
    }

#if IOS14

    private static void IOS14_AddPlistEntries(PlistElementDict rootDict)
    {
        if (FPG.AdPlacement.SkAdIds == null || FPG.AdPlacement.SkAdIds.Length <= 0) return;
        rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");

        PlistElementArray SKAdNetworkItemsArray = rootDict.CreateArray("SKAdNetworkItems");

        foreach (var skAdId in FPG.AdPlacement.SkAdIds)
        {
            PlistElementDict d = SKAdNetworkItemsArray.AddDict();
            d.SetString("SKAdNetworkIdentifier", skAdId);
        }
    }
#endif

    [PostProcessBuildAttribute(1)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        Debug.Log("ashlog: OnPostProcessBuild");
        
		if (target == BuildTarget.iOS)
		{

			PBXProject project = new PBXProject();
			string sPath = PBXProject.GetPBXProjectPath(path);
			project.ReadFromFile(sPath);

			//string tn = PBXProject.GetUnityTargetName();
			//string g = project.TargetGuidByName(tn);
            string g = project.GetUnityFrameworkTargetGuid();

            ModifyFrameworksSettings(project, g);

			// modify frameworks and settings as desired
			File.WriteAllText(sPath, project.WriteToString());
		}
    }

    static void EditUnityAppController(string pathToBuiltProject)
    {
        //Edit UnityAppController.mm
        //XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");
        //Refer to the header file of the third-party SDK
        //UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import \"ThirdDK.h\"");
        //add code:  [[ThirdSDK sharedInstance] showSplash:@"appkey" withWindow:self.window blockid:@"blockid"]; return YES;
        //string resultStr = "";
        //string newCodeStr = "    [[ThirdSDK sharedInstance] showSplash:@\"{0}\" withWindow:self.window blockid:@\"{1}\"];\n\n    return YES;";
        //resultStr = string.Format(newCodeStr, "appkey", "blockid");
        //UnityAppController.Replace("return YES;", resultStr, "didFinishLaunchingWithOptions");

        // Add InAppSearch code snippet to UnityAppController.mm file
        //XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");
        //UnityAppController.WriteBelow("#import \"UnityAppController.h\"", "#import \"SearchManager.h\"");
        //string code = "    [[SearchManager sharedManager] setUserActivityOnContinue:userActivity];\n    return YES;";
        //UnityAppController.Replace("return YES;", code, "continueUserActivity");
    }

    static void ModifyFrameworksSettings(PBXProject project, string g)
    {
        Debug.Log("ashlog: ModifyFrameworksSettings");

        // add hella frameworks

        Debug.Log(">> Automation, Frameworks... <<");

        //InAppSearch(Spotlight) frameworks
        //project.AddFrameworkToProject(g, "CoreSpotlight.framework", false);
        //project.AddFrameworkToProject(g, "MobileCoreServices.framework", false);

#if IOS14
        project.AddFrameworkToProject(g, "AppTrackingTransparency.framework", false);
#endif

        //project.AddFrameworkToProject(g, "libz.tbd", false);

        // go insane with build settings

        Debug.Log(">> Automation, Settings... <<");

		//project.AddBuildProperty(g,
		//	"LIBRARY_SEARCH_PATHS",
		//	"../blahblah/lib");

		//project.AddBuildProperty(g,
		//	"OTHER_LDFLAGS",
		//	"-lsblah -lbz2");

		// note that, due to some Apple shoddyness, you usually need to turn this off
		// to allow the project to ARCHIVE correctly (ie, when sending to testflight):
		project.AddBuildProperty(g, "ENABLE_BITCODE", "false");
        project.AddBuildProperty(g, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");

    }

    public class XClass : System.IDisposable
    {

        private string filePath;

        public XClass(string fPath)
        {
            filePath = fPath;
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError(filePath + "The file does not exist under the path!");
                return;
            }
        }
        public void Replace(string oldStr, string newStr, string method = "")
        {
            if (!File.Exists(filePath))
            {

                return;
            }
            bool getMethod = false;
            string[] codes = File.ReadAllLines(filePath);
            for (int i = 0; i < codes.Length; i++)
            {
                string str = codes[i].ToString();
                if (string.IsNullOrEmpty(method))
                {
                    if (str.Contains(oldStr)) codes.SetValue(newStr, i);
                }
                else
                {
                    if (!getMethod)
                    {
                        getMethod = str.Contains(method);
                    }
                    if (!getMethod) continue;
                    if (str.Contains(oldStr))
                    {
                        codes.SetValue(newStr, i);
                        break;
                    }
                }
            }
            File.WriteAllLines(filePath, codes);
        }


        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if (beginIndex == -1)
            {

                return;
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

            text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }
        public void Dispose()
        {

        }
    }
}
#endif
