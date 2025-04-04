using External.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace External.Storage
{
    public static class SaveManager
    {
        private static APIManager apiManager;
        private static SaveManagerIO SaveManagerIO = new SaveManagerIO();

        private static PlayerDatabase defaultFilePlayer;
        private static ShopDatabase defaultFileShop;
        private static UGCDatabase defaultFileUGC;

        private static PlayerDatabase _playerDatabase;
        public static PlayerDatabase playerDatabase
        {
            get
            {
                return _playerDatabase;
            }
            set
            {
                _playerDatabase = value;
                UpdatePlayerDatabase();
            }
        }

        private static ShopDatabase _shopDatabase;
        public static ShopDatabase shopDatabase
        {
            get
            {
                return _shopDatabase;
            }
            set
            {
                _shopDatabase = value;
                UpdateShopDatabase();
            }
        }

        private static UGCDatabase _ugcDatabase;

        public static UGCDatabase ugcDatabase
        {
            get
            {
                return _ugcDatabase;
            }
            set
            {
                _ugcDatabase = value;
            }
        }

        public static Action OnUpdateShopFile;
        public static Action OnUpdatePlayerFile;
        public static Action OnUpdateUGCFile;

        private static string pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
        private static string pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";
        public readonly static string pathToDirectoryMod = Application.dataPath +  "/Mods/";
        public readonly static string pathToDirectoryModExport = Application.dataPath + "/Mods/Export/";

        private static bool isInitialization = false;

        // ���������� ��� ����������� ������, ����� ������� ��� ��� ���������� ����
        // https://stackoverflow.com/questions/4364665/static-destructor
        private static readonly DestructorSaveManager Finalise = new DestructorSaveManager();

        private sealed class DestructorSaveManager
        {
            ~DestructorSaveManager()
            {
                isInitialization = false;
            }
        }

        // https://stackoverflow.com/questions/78536/deep-cloning-objects/78612#78612
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", nameof(source));

            if (ReferenceEquals(source, null)) return default;


            string json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void Init(APIManager apiManager, PlayerDatabase filePlayer, ShopDatabase fileShop, UGCDatabase ugcDatabase, PlayerDatabase defaultFilePlayer, ShopDatabase defaultFileShop, UGCDatabase defaultFileUGC)
        {
            SaveManager.apiManager = apiManager;
            SaveManager.playerDatabase = filePlayer;
            SaveManager.shopDatabase = fileShop;
            SaveManager.ugcDatabase = ugcDatabase;
            SaveManager.defaultFilePlayer = defaultFilePlayer;
            SaveManager.defaultFileShop = defaultFileShop;
            SaveManager.defaultFileUGC = defaultFileUGC;

            if (Application.isEditor && !isInitialization)
            {
                SaveManager.playerDatabase.JSONPlayer = Clone(SaveManager.defaultFilePlayer.JSONPlayer);
                SaveManager.shopDatabase.JSONShop = Clone(SaveManager.defaultFileShop.JSONShop);
                SaveManager.ugcDatabase.ugcPoints = Clone(SaveManager.defaultFileUGC.ugcPoints);
#if UNITY_EDITOR
                EditorApplication.playModeStateChanged += (state) =>
                {
                    if (state == PlayModeStateChange.ExitingPlayMode)
                    {
                        SaveManager.playerDatabase.JSONPlayer = new JSONPlayer();
                        SaveManager.shopDatabase.JSONShop = new JSONShop();
                        SaveManager.ugcDatabase.ugcPoints = new List<UGCPoint>();
                        isInitialization = false;
                    }
                };
#endif
                isInitialization = true;
                Debug.Log("SaveManager: ������� ��������������");
            }
            else
            {
                try
                {
                    if (_playerDatabase.JSONPlayer.resources.isPlayerRegistration == false)
                        _playerDatabase.JSONPlayer = SaveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

                    if (_playerDatabase.JSONPlayer == null)
                    {
                        _playerDatabase.JSONPlayer = new JSONPlayer();
                        _playerDatabase.JSONPlayer.resources = new ResourcePlayer();
                        SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, _playerDatabase.JSONPlayer);
                    }

                    if (_shopDatabase.JSONShop.nameShop == "")
                        _shopDatabase.JSONShop = SaveManagerIO.LoadJSONShop(pathToFileResourceShop);

                    if (_shopDatabase.JSONShop == null)
                    {
                        _shopDatabase.JSONShop = new JSONShop();
                        _shopDatabase.JSONShop.resources = new ResourceShop();
                        SaveManagerIO.SaveJSONShop(pathToFileResourceShop, _shopDatabase.JSONShop);
                    }

                    isInitialization = true;

                    Debug.Log("SaveManager: ������� ��������������");
                }
                catch
                {
                    Debug.LogError("SaveManager: ������ ������������");
                }
            }
        }

        public static async void RegistrationShop(string nameShop)
        {
            await Task.Run(() =>
            {
                if (playerDatabase.JSONPlayer.nameUser != "")
                {
                    shopDatabase.JSONShop.nameShop = nameShop;
                    if (shopDatabase.JSONShop.resources == null)
                        shopDatabase.JSONShop.resources = new ResourceShop();

                    shopDatabase.JSONShop.resources.isShopRegistration = true;

                    apiManager.RegistrationShop(playerDatabase.JSONPlayer.nameUser, nameShop, shopDatabase.JSONShop.resources);
                    apiManager.SetResourceShopPlayer(playerDatabase.JSONPlayer.nameUser, nameShop, shopDatabase.JSONShop.resources);

                    ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();
                    changedResources.Add("changedRegistration", playerDatabase.JSONPlayer.resources.isPlayerRegistration.ToString());

                    if (shopDatabase.JSONShop.resources.productSaves != null)
                    {
                        for (int i = 0; i < shopDatabase.JSONShop.resources.productSaves.Count; i++)
                        {
                            //changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].reward.typeReward.ToString());
                        }
                    }

                    resourceChangedShop.changedResources = changedResources;

                    //apiManager.CreateLogShop(playerDatabase.JSONPlayer.nameUser, nameShop, "������� ��� ��������������", resourceChangedShop);
                    UpdateShopDatabase();
                }
            });
        }

        public static async void RegistrationPlayer(string nameUser)
        {
            await Task.Run(() =>
            {
                playerDatabase.JSONPlayer.nameUser = nameUser;
                if (playerDatabase.JSONPlayer.resources == null)
                    playerDatabase.JSONPlayer.resources = new ResourcePlayer();

                playerDatabase.JSONPlayer.resources.isPlayerRegistration = true;

                apiManager.SetResourcePlayer(nameUser, playerDatabase.JSONPlayer.resources);
                ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                Dictionary<string, string> changedResources = new Dictionary<string, string>
            {
            { "changedRegistration", shopDatabase.JSONShop.resources.isShopRegistration.ToString() }
            };
                resourceChangedPlayer.changedResources = changedResources;

                apiManager.CreateLogPlayer(nameUser, "����� ��� ��������������", resourceChangedPlayer);
                UpdatePlayerDatabase();
            });
        }

        public static async void CreateGlobalEvent(GlobalEvent globalEvent)
        {
            await Task.Run(() =>
            {
                apiManager.CreateGlobalEvent(globalEvent);
            });
        }

        public static async void UpdateShopDatabase()
        {
            if (playerDatabase.JSONPlayer.nameUser != "" && shopDatabase.JSONShop.nameShop != "")
            {
                await Task.Run(() =>
                {
                    apiManager.SetResourceShopPlayer(playerDatabase.JSONPlayer.nameUser, shopDatabase.JSONShop.nameShop, shopDatabase.JSONShop.resources);

                    ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (shopDatabase.JSONShop.resources.productSaves != null)
                    {
                        changedResources.Add($"changedProductSaves", shopDatabase.JSONShop.resources.productSaves.Count.ToString());
                    }

                    resourceChangedShop.changedResources = changedResources;

                    //apiManager.CreateLogShop(playerDatabase.JSONPlayer.nameUser, shopDatabase.JSONShop.nameShop, "������ �������� ���� ��������", resourceChangedShop);
                    SaveManagerIO.SaveJSONShop(pathToFileResourceShop, shopDatabase.JSONShop);

                });
                OnUpdateShopFile?.Invoke();
            }
        }

        public static async void UpdateUGCDatabse()
        {
            await Task.Run(() =>
            {
                SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, playerDatabase.JSONPlayer);
            });
            OnUpdateUGCFile?.Invoke();
        }

        public static void ExportUGC(UGCPoint ugcPoint, string pathToFile)
        {
            SaveManagerIO.SaveUGCPoint(Path.GetFullPath(pathToFile), ugcPoint);
        }

        public static UGCPoint ImportUGC(string pathToUGC)
        {
           return SaveManagerIO.LoadUGCPoint(pathToUGC);
        }

        public static async void UpdatePlayerDatabase()
        {
            if (playerDatabase.JSONPlayer.nameUser != "")
            {
                await Task.Run(() =>
                {
                    apiManager.SetResourcePlayer(playerDatabase.JSONPlayer.nameUser, playerDatabase.JSONPlayer.resources);
                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (playerDatabase.JSONPlayer.resources != null)
                    {
                        if (playerDatabase.JSONPlayer.resources.magnets != null)
                            changedResources.Add($"changedMagnetSaves", playerDatabase.JSONPlayer.resources.magnets.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.products != null)
                        //    changedResources.Add($"changedProductSaves", filePlayer.JSONPlayer.resources.products.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.cellModelBoards != null)
                        //    changedResources.Add($"changedModelBoardSaves", filePlayer.JSONPlayer.resources.cellModelBoards.Count.ToString());
                        if (playerDatabase.JSONPlayer.resources.exercises != null)
                            changedResources.Add($"changedExerciseSaves", playerDatabase.JSONPlayer.resources.exercises.Count.ToString());
                        if (playerDatabase.JSONPlayer.resources.ingradients != null)
                            changedResources.Add($"changedIngradientSaves", playerDatabase.JSONPlayer.resources.ingradients.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.currentPickUpItem.typeItemFromTable != null)
                        //    changedResources.Add($"changedCurrentItem", filePlayer.JSONPlayer.resources.currentPickUpItem.typeItemFromTable.ToString());
                        changedResources.Add($"changedCurrentIndexDialog", playerDatabase.JSONPlayer.resources.currentIndexDialog.ToString());
                        changedResources.Add($"changedCurrentIndexDialogPoint", playerDatabase.JSONPlayer.resources.currentIndexDialogPoint.ToString());
                    }

                    apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "������ ������ ���� ��������", resourceChangedPlayer);
                    SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, playerDatabase.JSONPlayer);
                });
                OnUpdatePlayerFile?.Invoke();

            }
        }


        public static async void ChangeMagnetSave(MagnetData magnetSave)
        {
            await Task.Run(() =>
            {
                if (playerDatabase.JSONPlayer.nameUser != "")
                {
                    for (int j = 0; j < playerDatabase.JSONPlayer.resources.magnets.Count; j++)
                    {
                        if (playerDatabase.JSONPlayer.resources.magnets[j].typeMagnet == magnetSave.typeMagnet)
                        {
                            playerDatabase.JSONPlayer.resources.magnets[j] = magnetSave;
                            apiManager.SetResourcePlayer(playerDatabase.JSONPlayer.nameUser, playerDatabase.JSONPlayer.resources);

                            ResourceChangedPlayer resourceChangedPlayer1 = new ResourceChangedPlayer();
                            Dictionary<string, string> changedResources1 = new Dictionary<string, string>();

                            if (playerDatabase.JSONPlayer.resources.magnets != null)
                            {
                                for (int i = 0; i < playerDatabase.JSONPlayer.resources.magnets.Count; i++)
                                {
                                    changedResources1.Add($"changedMagnet_Magnet{i}", playerDatabase.JSONPlayer.resources.magnets[i].typeMagnet);
                                }
                            }

                            apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "������ ������ � �������� �� ������������ ���� ��������", resourceChangedPlayer1);
                            SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, playerDatabase.JSONPlayer);

                            return;
                        }
                    }

                    playerDatabase.JSONPlayer.resources.magnets.Add(magnetSave);
                    apiManager.SetResourcePlayer(playerDatabase.JSONPlayer.nameUser, playerDatabase.JSONPlayer.resources);

                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (playerDatabase.JSONPlayer.resources.magnets != null)
                    {
                        for (int i = 0; i < playerDatabase.JSONPlayer.resources.magnets.Count; i++)
                        {
                            changedResources.Add($"changedCountChangeProduct_Product{i}", playerDatabase.JSONPlayer.resources.magnets[i].typeMagnet);
                        }
                    }

                    apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "������ ������ � �������� �� ������������ ���� ��������", resourceChangedPlayer);
                    SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, playerDatabase.JSONPlayer);
                }
            });
        }


        public static void Printplayer()
        {
            PrintListPlayer();
        }
        public static void PrintShopInfo()
        {
            PrintListShop(playerDatabase.JSONPlayer.nameUser);
        }


        private static async void PrintListShop(string userName)
        {
            await apiManager.GetListShopPlayer(userName);
        }

        private static async void PrintListLogsShop(string userName, string shopName)
        {
            await apiManager.GetListLogsShop(userName, shopName);
        }

        private static async void PrintListPlayer()
        {
            await apiManager.GetListPlayers();
        }
        private static async void PrintListLogPlayer(string userName)
        {
            await apiManager.GetListLogsPlayer(userName);
        }

        public static async void DeleteListPlayer()
        {
            List<JSONPlayer> jSONPlayers = await apiManager.GetListPlayers();
            for (int i = 0; i < jSONPlayers.Count; i++)
            {
                apiManager.DeletePlayer(jSONPlayers[i].nameUser);
            }
        }

        public static void ResetFilePlayer()
        {
            SaveManager.playerDatabase.JSONPlayer = Clone(SaveManager.defaultFilePlayer.JSONPlayer);
        }

        public static void ResetFileShop()
        {
            SaveManager.shopDatabase.JSONShop = Clone(SaveManager.defaultFileShop.JSONShop);
        }

        public static void ResetFileUGC()
        {
            SaveManager.ugcDatabase.ugcPoints = Clone(SaveManager.defaultFileUGC.ugcPoints);
        }
    }
}
