using External.API;
using Game.Environment.Fridge;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace External.Storage
{
    public class SaveManager
    {
        private APIManager apiManager;
        private SaveManagerIO saveManagerIO;

        public FilePlayer defaultFilePlayer;
        public FileShop defaultFileShop;

        public FilePlayer filePlayer; 
        public FileShop fileShop;

        private string pathToFileResourcePlayer;
        private string pathToFileResourceShop;

        public void Init(APIManager apiManager, FilePlayer filePlayer, FileShop fileShop, FilePlayer defaultFilePlayer, FileShop defaultFileShop)
        {
            this.apiManager = apiManager;
            this.filePlayer = filePlayer;
            this.fileShop = fileShop;
            this.defaultFilePlayer = defaultFilePlayer;
            this.defaultFileShop = defaultFileShop;

#if UNITY_EDITOR
            this.filePlayer = this.defaultFilePlayer;
            this.fileShop = this.defaultFileShop;
#else
            
#endif


            try
            {
                saveManagerIO = new SaveManagerIO();

                pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
                pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";

                if (filePlayer.JSONPlayer.resources.isPlayerRegistration == false)
                    filePlayer.JSONPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

                if (filePlayer.JSONPlayer == null)
                {
                    filePlayer.JSONPlayer = new JSONPlayer();
                    filePlayer.JSONPlayer.resources = new ResourcePlayer();
                    saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }

                if (fileShop.JSONShop.nameShop == "")
                    fileShop.JSONShop = saveManagerIO.LoadJSONShop(pathToFileResourceShop);

                if (fileShop.JSONShop == null)
                {
                    fileShop.JSONShop = new JSONShop();
                    fileShop.JSONShop.resources = new ResourceShop();
                    saveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
                }
            }
            catch
            {
                Debug.LogError("SaveManager: Ошибка иницилизации");
            }

            Debug.Log("SaveManager: Успешно иницилизирован");
        }

        public void Init(APIManager apiManager, FilePlayer filePlayer)
        {
            this.filePlayer = filePlayer;
            this.apiManager = apiManager;

            saveManagerIO = new SaveManagerIO();

            pathToFileResourcePlayer = Application.persistentDataPath + $"/rp.buf";
            pathToFileResourceShop = Application.persistentDataPath + $"/rs.buf";

            if (filePlayer.JSONPlayer.resources.isPlayerRegistration == false)
                filePlayer.JSONPlayer = saveManagerIO.LoadJSONPlayer(pathToFileResourcePlayer);

            if (filePlayer.JSONPlayer == null)
            {
                filePlayer.JSONPlayer = new JSONPlayer();
                filePlayer.JSONPlayer.resources = new ResourcePlayer();
                saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
            }
        }

        public async void RegistrationShop(string nameShop)
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
                            changedResources.Add($"changedCountProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].reward.typeReward.ToString());
                        }
                    }

                    resourceChangedShop.changedResources = changedResources;

                    apiManager.CreateLogShop(filePlayer.JSONPlayer.nameUser, nameShop, "Магазин был иницилизирован", resourceChangedShop);
                    UpdateShopFile();
                }
            });
        }

        public async void RegistrationPlayer(string nameUser)
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

        public async void UpdateShopFile()
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
                    saveManagerIO.SaveJSONShop(pathToFileResourceShop, fileShop.JSONShop);
                }
            });
        }

        public async void UpdatePlayerFile()
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
                        if (filePlayer.JSONPlayer.resources.magnetSaves != null)
                            changedResources.Add($"changedMagnetSaves", filePlayer.JSONPlayer.resources.magnetSaves.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.products != null)
                            changedResources.Add($"changedProductSaves", filePlayer.JSONPlayer.resources.products.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.modelBoardSaves != null)
                            changedResources.Add($"changedModelBoardSaves", filePlayer.JSONPlayer.resources.modelBoardSaves.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.exerciseSaves != null)
                            changedResources.Add($"changedExerciseSaves", filePlayer.JSONPlayer.resources.exerciseSaves.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.ingradientSaves != null)
                            changedResources.Add($"changedIngradientSaves", filePlayer.JSONPlayer.resources.ingradientSaves.Count.ToString());
                        if (filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable != null)
                            changedResources.Add($"changedCurrentItem", filePlayer.JSONPlayer.resources.currentItemFromTableSave.typeItemFromTable.ToString());
                        changedResources.Add($"changedCurrentIndexDialog", filePlayer.JSONPlayer.resources.currentIndexDialog.ToString());
                        changedResources.Add($"changedCurrentIndexDialogPoint", filePlayer.JSONPlayer.resources.currentIndexDialogPoint.ToString());
                    }

                    apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer);
                    saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }
            });
        }

        public async void ChangeSaveTypeProduct(SaveTypeProduct saveTypeProduct)
        {
            await Task.Run(() =>
            {
                if (filePlayer.JSONPlayer.nameUser != "")
                {
                    for (int j = 0; j < filePlayer.JSONPlayer.resources.products.Count; j++)
                    {
                        if (filePlayer.JSONPlayer.resources.products[j].typeProduct == saveTypeProduct.typeProduct)
                        {
                            filePlayer.JSONPlayer.resources.products[j].countProduct += saveTypeProduct.countProduct;
                            UpdatePlayerFile();

                            apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);
                            ResourceChangedPlayer resourceChangedPlayer1 = new ResourceChangedPlayer();
                            Dictionary<string, string> changedResources1 = new Dictionary<string, string>();

                            apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока были изменены", resourceChangedPlayer1);
                            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);

                            return;
                        }
                    }

                    SaveTypeProduct saveType = new SaveTypeProduct();
                    saveType.typeProduct = saveTypeProduct.typeProduct;
                    saveType.countProduct += saveTypeProduct.countProduct;

                    filePlayer.JSONPlayer.resources.products.Add(saveType);
                    apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);
                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (fileShop.JSONShop.resources.productSaves != null)
                    {
                        for (int i = 0; i < fileShop.JSONShop.resources.productSaves.Count; i++)
                        {
                            changedResources.Add($"changedCountChangeProduct_Product{i}", fileShop.JSONShop.resources.productSaves[i].reward.typeReward.ToString());
                        }
                    }

                    apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока о покупках были изменены", resourceChangedPlayer);
                    saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }
            });
        }

        public async void ChangeMagnetSave(MagnetSave magnetSave)
        {
            await Task.Run(() =>
            {
                if (filePlayer.JSONPlayer.nameUser != "")
                {
                    for (int j = 0; j < filePlayer.JSONPlayer.resources.magnetSaves.Count; j++)
                    {
                        if (filePlayer.JSONPlayer.resources.magnetSaves[j].typeMagnet == magnetSave.typeMagnet)
                        {
                            filePlayer.JSONPlayer.resources.magnetSaves[j] = magnetSave;
                            apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);

                            ResourceChangedPlayer resourceChangedPlayer1 = new ResourceChangedPlayer();
                            Dictionary<string, string> changedResources1 = new Dictionary<string, string>();

                            if (filePlayer.JSONPlayer.resources.magnetSaves != null)
                            {
                                for (int i = 0; i < filePlayer.JSONPlayer.resources.magnetSaves.Count; i++)
                                {
                                    changedResources1.Add($"changedMagnet_Magnet{i}", filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet);
                                }
                            }

                            apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer1);
                            saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);

                            return;
                        }
                    }

                    filePlayer.JSONPlayer.resources.magnetSaves.Add(magnetSave);
                    apiManager.SetResourcePlayer(filePlayer.JSONPlayer.nameUser, filePlayer.JSONPlayer.resources);

                    ResourceChangedPlayer resourceChangedPlayer = new ResourceChangedPlayer();
                    Dictionary<string, string> changedResources = new Dictionary<string, string>();

                    if (filePlayer.JSONPlayer.resources.magnetSaves != null)
                    {
                        for (int i = 0; i < filePlayer.JSONPlayer.resources.magnetSaves.Count; i++)
                        {
                            changedResources.Add($"changedCountChangeProduct_Product{i}", filePlayer.JSONPlayer.resources.magnetSaves[i].typeMagnet);
                        }
                    }

                    apiManager.CreateLogPlayer(filePlayer.JSONPlayer.nameUser, "Данные игрока о магнитах на холодильнике были изменены", resourceChangedPlayer);
                    saveManagerIO.SaveJSONPlayer(pathToFileResourcePlayer, filePlayer.JSONPlayer);
                }
            });
        }


        public void Printplayer()
        {
            PrintListPlayer();
        }
        public void PrintShopInfo()
        {
            PrintListShop(filePlayer.JSONPlayer.nameUser);
        }


        private async void PrintListShop(string userName)
        {
            await apiManager.GetListShopPlayer(userName);
        }

        private async void PrintListLogsShop(string userName, string shopName)
        {
            await apiManager.GetListLogsShop(userName, shopName);
        }

        private async void PrintListPlayer()
        {
            await apiManager.GetListPlayers();
        }
        private async void PrintListLogPlayer(string userName)
        {
            await apiManager.GetListLogsPlayer(userName);
        }

        public async void DeleteListPlayer()
        {
            List<JSONPlayer> jSONPlayers = await apiManager.GetListPlayers();
            for (int i = 0; i < jSONPlayers.Count; i++)
            {
                apiManager.DeletePlayer(jSONPlayers[i].nameUser);
            }
        }

        public JSONPlayer GetJSONPlayer()
        {
            return filePlayer.JSONPlayer;
        }
    }
}
