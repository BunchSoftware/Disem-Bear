using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.PackageManager;

public class HotReloadHuck : MonoBehaviour
{
    static class PackageConst
    {
        //CI changes this property to 'true' for asset store builds.
        //Don't touch unless you know what you are doing
        public static bool IsAssetStoreBuild => false;


        public const string Version = "1.12.14";
        // Never higher than Version
        // Used for the download
        public const string ServerVersion = "1.12.12";
        public const string PackageName = "com.singularitygroup.hotreload";
        public const string LibraryCachePath = "Library/" + PackageName;
        public const string ConfigFileName = "hot-reload-config.json";
    }

    public enum RegistrationOutcome
    {
        None,
        Indie,
        Business,
    }

    public const string registerOutcomePath = PackageConst.LibraryCachePath + "/registerOutcome.txt";
    private const string registerFlagPath = PackageConst.LibraryCachePath + "/registerFlag.txt";

    public void StartRegistration()
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        Directory.CreateDirectory(Path.GetDirectoryName(registerFlagPath));
        using (File.Create(registerFlagPath)) { }
    }

    public void FinishRegistration(RegistrationOutcome outcome)
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        Directory.CreateDirectory(Path.GetDirectoryName(registerFlagPath));
        File.WriteAllText(registerOutcomePath, outcome.ToString());
        File.Delete(registerFlagPath);
    }

    private void Start()
    {
       StartRegistration();
       FinishRegistration(RegistrationOutcome.Business);
       Debug.Log("HOT RELOAD HACK");
    }
}
