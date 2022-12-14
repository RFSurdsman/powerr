using Mirror;
using UnityEngine;
using System;

namespace Powerr.Infrastructure
{
    /// <summary>
    /// Customised version of the Network Manager for our needs.
    /// </summary>
    public class PowerrNetworkManager : NetworkManager
    {
        public new static PowerrNetworkManager singleton => NetworkManager.singleton as PowerrNetworkManager;

        [Header("Other")]
        [SerializeField] UnityEngine.Object[] destroyOnHeadlessLoad;
        [SerializeField] UnityEngine.Object[] destroyOnClientLoad;
        [SerializeField] EditorDebugModeEnum editorDebugMode;

        bool IsHeadless => SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
        const string localServerAddress = "localhost";
        enum EditorDebugModeEnum
        {
            Host,
            Headless,
            ClientLocal,
            ClientOnline
        }

        /// <summary>
        /// On game start, initialise the networking infrastructure. If you are running in the Unity Editor, we will
        /// use debug mode, which allows you to test the game in various environments without needing to deploy to AWS.
        /// </summary>
        public override void Start()
        {
            if (Application.isEditor)
            {
                InitialiseDebugModeNetworking();
            }
            else
            {
                InitialiseNetworking();
            }
        }

        /// <summary>
        /// Initialises networking in debug mode, intended for use when working with the game in the Unity Editor. You may
        /// select the desired debugging mode via the dropdown in the inspector.
        /// </summary>
        /// <remarks>
        /// Possible debugging modes:
        /// Host - Start as both the server and the client.
        /// Headless - Headless server mode (<see cref="StartHeadless"/>).
        /// Client Local - Client connects to localhost instead of the specified networkAddress, so that we can connect to 
        /// another editor which is running in Host or Headless mode.
        /// Client Online - Client connects to the online server and acts as a client, so that we can play on racerr.io 
        /// through the Unity Editor.
        /// </remarks>
        void InitialiseDebugModeNetworking()
        {
            switch (editorDebugMode)
            {
                case EditorDebugModeEnum.Host: StartHost(); break;
                case EditorDebugModeEnum.Headless: StartHeadless(); break;
                case EditorDebugModeEnum.ClientOnline: StartClient(); break;
                case EditorDebugModeEnum.ClientLocal: networkAddress = localServerAddress; StartClient(); break;
                default: throw new InvalidOperationException("Invalid Unity Editor Debug Mode attempt: " + editorDebugMode);
            }
        }

        /// <summary>
        /// Initialises networking for the compiled version of the game. If we have no graphics card (i.e. having no head), 
        /// we will assume we are a headless server (<see cref="StartHeadless"/>), otherwise we are a client.
        /// </summary>
        void InitialiseNetworking()
        {
            if (IsHeadless)
            {
                StartHeadless();
            }
            else
            {
                StartClient();
            }
        }

        /// <summary>
        /// Special mode designed especially for deployment to a server. This will destroy all Unity Engine objects
        /// which are only useful for the client (such as the UI) to optimise server performance.
        /// We also limit the frame rate, as there is no point having the frame rate of the server be higher
        /// than the server update frequency.
        /// </summary>
        void StartHeadless()
        {
            foreach (UnityEngine.Object unityEngineObject in destroyOnHeadlessLoad)
            {
                Destroy(unityEngineObject);
            }

            // Create fake audio listener to prevent annoying log messages caused by deleting the camera.
            new GameObject("Audio Listener for Headless mode").AddComponent<AudioListener>();

            Application.targetFrameRate = serverTickRate;
            StartHost();
        }

        /// <summary>
        /// When starting the game as a client only, destroy Unity Engine objects which are
        /// only useful for the server to optimise performance.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                foreach (UnityEngine.Object unityEngineObject in destroyOnClientLoad)
                {
                    Destroy(unityEngineObject);
                }
            }
        }

        /// <summary>
        /// Upon player joining, add the new player, associate them with the Player game object and synchronise on all clients.
        /// Note we don't add a player for the headless server player - they are not a real player.
        /// </summary>
        /// <param name="conn">Player's connection info.</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if ((IsHeadless || Application.isEditor && editorDebugMode == EditorDebugModeEnum.Headless) && conn == NetworkServer.localConnection)
            {
                return;
            }

            GameObject player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        /// <summary>
        /// Upon player disconnect, delete the player, remove the Player game object and synchronise on all clients.
        /// </summary>
        /// <param name="conn">Player's connection info.</param>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            NetworkServer.DestroyPlayerForConnection(conn);
        }
    }
}
