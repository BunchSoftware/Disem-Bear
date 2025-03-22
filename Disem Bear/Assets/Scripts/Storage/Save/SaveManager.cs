using External.API;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static Action OnUpdateShopFile;
        public static Action OnUpdatePlayerFile;

        private static string pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
        private static string pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";

        private static bool isInitialization = false;

        // Деструктор для стачиеского класса, чтобы удалить его при выключении игры
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

            using var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }

        public static void Init(APIManager apiManager, PlayerDatabase filePlayer, ShopDatabase fileShop, PlayerDatabase defaultFilePlayer, ShopDatabase defaultFileShop)
        {
            SaveManager.apiManager = apiManager;
            SaveManager.playerDatabase = filePlayer;
            SaveManager.shopDatabase = fileShop;
            SaveManager.defaultFilePlayer = defaultFilePlayer;
            SaveManager.defaultFileShop = defaultFileShop;

            if (Application.isEditor && !isInitialization)
            {
                SaveManager.playerDatabase.JSONPlayer = Clone(SaveManager.defaultFilePlayer.JSONPlayer);
                SaveManager.shopDatabase.JSONShop = Clone(SaveManager.defaultFileShop.JSONShop);
#if UNITY_EDITOR
                EditorApplication.playModeStateChanged += (state) =>
                {
                    if (state == PlayModeStateChange.ExitingPlayMode)
                    {
                        SaveManager.playerDatabase.JSONPlayer = new JSONPlayer();
                        SaveManager.shopDatabase.JSONShop = new JSONShop();
                        isInitialization = false;
                    }
                };
#endif
                isInitialization = true;
                Debug.Log("SaveManager: Успешно иницилизирован");
            }
            else
            {
                try
                {
                    if (filePlayer.JSONPlayer.resources.isPlayerRegistration == false)
                        filePlayer.JSONPlayer = SaveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

                    if (filePlayer.JSONPlayer == null)
                    {
                        filePlayer.JSONPlayer = new JSONPlayer();
                        filePlayer.JSONPlayer.resources = new ResourcePlayer();
                        SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                    }

                    if (fileShop.JSONShop.nameShop == "")
                        fileShop.JSONShop = SaveManagerIO.LoadJSONShop(pathToFileResourceShop);

                    if (fileShop.JSONShop == null)
                    {
                        fileShop.JSONShop = new JSONShop();
                        fileShop.JSONShop.resources = new ResourceShop();
                        SaveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
                    }

                    isInitialization = true;

                    Debug.Log("SaveManager: Успешно иницилизирован");
                }
                catch
                {
                    Debug.LogError("SaveManager: Ошибка иницилизации");
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

                    apiManager.CreateLogShop(playerDatabase.JSONPlayer.nameUser, nameShop, "Магазин был иницилизирован", resourceChangedShop);
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


                apiManager.RegistrationPlayer(nameUser, playerDatabase.JSONPlayer.resources);
                apiManager.SetResourcePlayer(nameUser, playerDatabase.JSONPlayer.resources);
                ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                Dictionary<string, string> changedResources = new Dictionary<string, string>
            {
            { "changedRegistration", shopDatabase.JSONShop.resources.isShopRegistration.ToString() }
            };
                resourceChangedPlayer.changedResources = changedResources;

                apiManager.CreateLogPlayer(nameUser, "Игрок был иницилизирован", resourceChangedPlayer);
                UpdatePlayerDatabase();
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

                    apiManager.CreateLogShop(playerDatabase.JSONPlayer.nameUser, shopDatabase.JSONShop.nameShop, "Данные магазина были изменены", resourceChangedShop);
                    SaveManagerIO.SaveJSONShop(pathToFileResourceShop, shopDatabase.JSONShop);

                });
                OnUpdateShopFile?.Invoke();
            }
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

                    apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer);
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

                            apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer1);
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

                    apiManager.CreateLogPlayer(playerDatabase.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer);
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
    }
}
