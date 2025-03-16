using External.API;
using Game.Environment.Fridge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace External.Storage
{
    public static class SaveManager
    {
        private static APIManager apiManager;
        private static SaveManagerIO SaveManagerIO = new SaveManagerIO();

        private static FilePlayer defaultFilePlayer;
        private static FileShop defaultFileShop;


        private static FilePlayer _filePlayer;
        public static FilePlayer filePlayer 
        {
            get
            {
                return _filePlayer;
            }
            set 
            {
                _filePlayer = value;
                UpdatePlayerFile();
            }
        }

        private static FileShop _fileShop;
        public static FileShop fileShop 
        {
            get
            {
                return _fileShop;
            }
            set
            {
                _fileShop = value;
                UpdateShopFile();
            }
        }

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

        public static void Init(APIManager apiManager, FilePlayer filePlayer, FileShop fileShop, FilePlayer defaultFilePlayer, FileShop defaultFileShop)
        {
            SaveManager.apiManager = apiManager;
            SaveManager.filePlayer = filePlayer;
            SaveManager.fileShop = fileShop;
            SaveManager.defaultFilePlayer = defaultFilePlayer;
            SaveManager.defaultFileShop = defaultFileShop;

            if(Application.isEditor && !isInitialization)
            {
                SaveManager.filePlayer.JSONPlayer = Clone(SaveManager.defaultFilePlayer.JSONPlayer);
                SaveManager.fileShop.JSONShop = Clone(SaveManager.defaultFileShop.JSONShop);
                EditorApplication.playModeStateChanged += (state) =>
                {
                    if (state == PlayModeStateChange.ExitingPlayMode)
                    {
                        SaveManager.filePlayer.JSONPlayer = new JSONPlayer();
                        SaveManager.fileShop.JSONShop = new JSONShop();
                        isInitialization = false;
                    }
                };
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
                if (filePlayer.JSONPlayer.nameUser != "")
                {
                    fileShop.JSONShop.nameShop = nameShop;
                    if (fileShop.JSONShop.resources == null)
                        fileShop.JSONShop.resources = new ResourceShop();

                    fileShop.JSONShop.resources.isShopRegistration = true;

                    apiManager.RegistrationShop(filePlayer.JSONPlayer.nameUser, nameShop, fileShop.JSONShop.resources);
                    apiManager.SetResourceShopPlayer(filePlayer.JSONPlayer.nameUser, nameShop, fileShop.JSONShop.resources);

                    ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();
                    changedResources.Add("changedRegistration", filePlayer.JSONPlayer.resources.isPlayerRegistration.ToString());

                    if (fileShop.JSONShop.resources.productSaves != null)
                    {
                        for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
                        {
                            //changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].reward.typeReward.ToString());
                        }
                    }

                    resourceChangedShop.changedResources = changedResources;

                    apiManager.CreateLogShop(filePlayer.JSONPlayer.nameUser, nameShop, "Магазин был иницилизирован", resourceChangedShop);
                    UpdateShopFile();
                }
            });
        }

        public static async void RegistrationPlayer(string nameUser)
        {
            await Task.Run(() =>
            {
                filePlayer.JSONPlayer.nameUser = nameUser;
                if (filePlayer.JSONPlayer.resources == null)
                    filePlayer.JSONPlayer.resources = new ResourcePlayer();

                filePlayer.JSONPlayer.resources.isPlayerRegistration = true;


                apiManager.RegistrationPlayer(nameUser, filePlayer.JSONPlayer.resources);
                apiManager.SetResourcePlayer(nameUser, filePlayer.JSONPlayer.resources);
                ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                Dictionary<string, string> changedResources = new Dictionary<string, string>
            {
            { "changedRegistration", fileShop.JSONShop.resources.isShopRegistration.ToString() }
            };
                resourceChangedPlayer.changedResources = changedResources;

                apiManager.CreateLogPlayer(nameUser, "Игрок был иницилизирован", resourceChangedPlayer);
                UpdatePlayerFile();
            });
        }

        private static async void UpdateShopFile()
        {
            await Task.Run(() =>
            {
                if (filePlayer.JSONPlayer.nameUser != "" && fileShop.JSONShop.nameShop != "")
                {
                    apiManager.SetResourceShopPlayer(filePlayer.JSONPlayer.nameUser, fileShop.JSONShop.nameShop, fileShop.JSONShop.resources);

                    ResourceChangedShop resourceChangedShop = new ResourceChangedShop();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (fileShop.JSONShop.resources.productSaves != null)
                    {
                        changedResources.Add($"changedProductSaves", fileShop.JSONShop.resources.productSaves.Count.ToString());
                    }

                    resourceChangedShop.changedResources = changedResources;

                    apiManager.CreateLogShop(filePlayer.JSONPlayer.nameUser, fileShop.JSONShop.nameShop, "Данные магазина были изменены", resourceChangedShop);
                    SaveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
                }
            });
        }

        private static async void UpdatePlayerFile()
        {
            await Task.Run(() =>
            {
                if (filePlayer.JSONPlayer.nameUser != "")
                {
                    apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);
                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (filePlayer.JSONPlayer.resources != null)
                    {
                        if (filePlayer.JSONPlayer.resources.magnets != null)
                            changedResources.Add($"changedMagnetSaves", filePlayer.JSONPlayer.resources.magnets.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.products != null)
                        //    changedResources.Add($"changedProductSaves", filePlayer.JSONPlayer.resources.products.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.cellModelBoards != null)
                        //    changedResources.Add($"changedModelBoardSaves", filePlayer.JSONPlayer.resources.cellModelBoards.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.exercises != null)
                            changedResources.Add($"changedExerciseSaves", filePlayer.JSONPlayer.resources.exercises.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.ingradients != null)
                            changedResources.Add($"changedIngradientSaves", filePlayer.JSONPlayer.resources.ingradients.Count.ToString());
                        //if (filePlayer.JSONPlayer.resources.currentPickUpItem.typeItemFromTable != null)
                        //    changedResources.Add($"changedCurrentItem", filePlayer.JSONPlayer.resources.currentPickUpItem.typeItemFromTable.ToString());
                        changedResources.Add($"changedCurrentIndexDialog", filePlayer.JSONPlayer.resources.currentIndexDialog.ToString());
                        changedResources.Add($"changedCurrentIndexDialogPoint", filePlayer.JSONPlayer.resources.currentIndexDialogPoint.ToString());
                    }

                    apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer);
                    SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }
            });
        }

        public static async void ChangeMagnetSave(MagnetData magnetSave)
        {
            await Task.Run(() =>
            {
                if (filePlayer.JSONPlayer.nameUser != "")
                {
                    for (int j = 0; j < filePlayer.JSONPlayer.resources.magnets.Count; j++)
                    {
                        if (filePlayer.JSONPlayer.resources.magnets[j].typeMagnet == magnetSave.typeMagnet)
                        {
                            filePlayer.JSONPlayer.resources.magnets[j] = magnetSave;
                            apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);

                            ResourceChangedPlayer resourceChangedPlayer1 = new ResourceChangedPlayer();
                            Dictionary<string, string> changedResources1 = new Dictionary<string, string>();

                            if (filePlayer.JSONPlayer.resources.magnets != null)
                            {
                                for (int i = 0; i < filePlayer.JSONPlayer.resources.magnets.Count; i++)
                                {
                                    changedResources1.Add($"changedMagnet_Magnet{i}", filePlayer.JSONPlayer.resources.magnets[i].typeMagnet);
                                }
                            }

                            apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer1);
                            SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);

                            return;
                        }
                    }

                    filePlayer.JSONPlayer.resources.magnets.Add(magnetSave);
                    apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);

                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (filePlayer.JSONPlayer.resources.magnets != null)
                    {
                        for (int i = 0; i < filePlayer.JSONPlayer.resources.magnets.Count; i++)
                        {
                            changedResources.Add($"changedCountChangeProduct_Product{i}", filePlayer.JSONPlayer.resources.magnets[i].typeMagnet);
                        }
                    }

                    apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer);
                    SaveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }
            });
        }


        public static void Printplayer()
        {
            PrintListPlayer();
        }
        public static void PrintShopInfo()
        {
            PrintListShop(filePlayer.JSONPlayer.nameUser);
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
            SaveManager.filePlayer.JSONPlayer = Clone(SaveManager.defaultFilePlayer.JSONPlayer);
        }

        public static void ResetFileShop()
        {
            SaveManager.fileShop.JSONShop = Clone(SaveManager.defaultFileShop.JSONShop);
        }
    }
}
