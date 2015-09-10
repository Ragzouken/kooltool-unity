using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using System.Linq;

using Newtonsoft.Json;
using kooltool.Serialization;
using Ionic.Zip;
using kooltool.Editor.Modes;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviour
    {
        public static Sprite debug;
        public Image Debug_;
        public RectTransform Debug2;

        [Header("Camera / Canvas")]
        [SerializeField] private GraphicRaycaster worldRaycaster; 

        [Header("Toolbar")]
        [SerializeField] protected Button playButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button exportButton;

        [SerializeField] private GameObject browserLayer;

        [SerializeField] protected WorldCamera WCamera;
        [SerializeField] protected Camera Camera_;
        
        [SerializeField] protected kooltool.Player.Player Player;

        public HighlightGroup Highlights;

        [Header("UI")]
        [SerializeField] protected Slider ZoomSlider;
        [SerializeField] public Tooltip tooltip;

        [Header("Cursors")]
        [SerializeField] protected GameObject Cursors;
        [SerializeField] protected PixelCursor PixelCursor;
        [SerializeField] protected TileCursor TileCursor;
        [SerializeField] public Image toolIcon;

        [Header("Settings")]
        [SerializeField] protected AnimationCurve ZoomCurve;

        public Toolbox Toolbox;

        public Serialization.Project project_;

        public MapGenerator generator;

        public Layer Layer;

        public float Zoom { get; protected set; }

        protected Coroutine ZoomCoroutine;

        // poop
        Vector2 pansite;
        protected bool Panning;
        public Project Project;

        #region Modes

        private Modes.Object objectMode;
        private Modes.Draw drawMode;
        private Modes.Tile tileMode;

        private readonly Stack<Modes.Mode> modes = new Stack<Mode>();

        private Modes.Mode currentMode
        {
            get
            {
                return modes.Peek();
            }
        }

        private void PushMode(Modes.Mode mode)
        {
            currentMode.Exit();
            modes.Push(mode);
            currentMode.Enter();
        }

        private void PopMode()
        {
            currentMode.Exit();
            modes.Pop();
            currentMode.Enter();
        }

        private void SetMode(Modes.Mode mode)
        {
            currentMode.Exit();
            modes.Clear();
            modes.Push(mode);
            currentMode.Enter();
        }

        #endregion

        public static Color GetFlashColour(float alpha=1f)
        {
            float hue = (Time.timeSinceLevelLoad / 0.5f) % 1f;
            
            IList<double> RGB = HUSL.HUSLPToRGB(new double[] { hue * 360, 100, 75 });
            
            return new Color((float) RGB[0], (float) RGB[1], (float) RGB[2], alpha);
        }

        public readonly List<Editable> hovered = new List<Editable>();

        public bool ShowCursors
        {
            get
            {
                return !Panning
                    && !Input.GetKey(KeyCode.Tab)
                    && IsPointerOverWorld();
            }
        }

        public bool IsPointerOverWorld()
        {
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            var results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointer, results);

            return !(results.Count > 0 && results[0].gameObject.layer != LayerMask.NameToLayer("World"));
        }

        public void SetPixelTool()
        {
            SetMode(drawMode);
        }
        
        public void SetTileTool()
        {
            SetMode(tileMode);
        }

        public void SetProject(Serialization.Project project)
        {
            project_ = project;

            Layer.SetLayer(project.world.layers[0]);
        }

        public Browser browser;

        private bool FindEmbedded()
        {
            if (System.IO.File.Exists(Application.dataPath + "/autoplay/summary.json"))
            {
                var summary = ProjectTools.LoadSummary("autoplay", Application.dataPath);
                var project = ProjectTools.LoadProject(summary);
                SetProject(project);

                return true;
            }

            return false;
        }

        protected void Awake()
        {

            Project = new Project(new Point(32, 32));
                 
            SetProject(Serialization.ProjectTools.Blank());
            project_.tileset.TestTile();
            //SetProject(LoadProject("test"));

            objectMode = new Modes.Object(this);
            drawMode = new Modes.Draw(this, PixelCursor);
            tileMode = new Modes.Tile(this, TileCursor);

            Toolbox.PixelTab.SetPixelTool(drawMode);
            Toolbox.TileTab.SetTileTool(tileMode);

            modes.Push(drawMode);

            // poop
            TileCursor.mode = tileMode;
            PixelCursor.mode = drawMode;

            ZoomTo(1f);

            playButton.onClick.AddListener(Play);
            saveButton.onClick.AddListener(Save);
            exportButton.onClick.AddListener(Export);

            browser.OnConfirmed += delegate(Serialization.Summary summary)
            {
                browser.gameObject.SetActive(false);
                browserLayer.SetActive(false);
                SetProject(Serialization.ProjectTools.LoadProject(summary));
            };

            browser.Refresh();

            if (FindEmbedded()) browser.gameObject.SetActive(false);
        }

        protected void Play()
        {
            EventSystem.current.SetSelectedGameObject(null);

            foreach (var character in Layer.Characters.Instances)
            {
                character.SetPlayer();
            }

            ZoomTo(0);
            Player.Setup(project_);
        }

        private void Save()
        {
            project_.index.Save(project_);

            var summary = new Serialization.Summary
            {
                title = project_.index.folder,
                description = "unset",
                icon = "icon.png",
                iconSprite = PixelDraw.Brush.Circle(128, new Color(Random.value, Random.value, Random.value, 1)),
                folder = project_.index.folder,
                root = project_.index.root,
            };

            Serialization.ProjectTools.SaveSummary(summary);
        }

#if UNITY_EDITOR
        private static string exepath = "../kooltool/";
#else
        private static string exepath = "../..";
#endif
        private void Export()
        {
            Save();
            
            var name = project_.index.folder;

            using (ZipFile zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = -1;
                zip.AddDirectory(Application.dataPath + "/" + exepath + "/windows", name);
                zip.AddDirectory(project_.index.path, name + "/kooltool_Data/autoplay");

                zip.Save(Application.persistentDataPath + "/" + name + "-windows.zip");
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = -1;
                zip.AddDirectory(Application.dataPath + "/" + exepath + "/linux", name);
                zip.AddDirectory(project_.index.path, name + "/kooltool_Data/autoplay");

                zip.Save(Application.persistentDataPath + "/" + name + "-linux.zip");
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = -1;
                zip.AddDirectory(Application.dataPath + "/" + exepath + "/mac/kooltool.app", name + ".app");
                zip.AddDirectory(project_.index.path, name + ".app/Contents/autoplay");

                zip.Save(Application.persistentDataPath + "/" + name + "-mac.zip");
            }
        }

        private void Test1(object a, AddProgressEventArgs args)
        {
            Debug.Log(args.BytesTransferred + " / " + args.TotalBytesToTransfer);
        }

        private void Test2(object a, SaveProgressEventArgs args)
        {
            Debug.Log(args.BytesTransferred + " / " + args.TotalBytesToTransfer);
        }

        protected void Start()
        {
            //SetProject(Project);

            Toolbox.Hide();
        }

        protected bool GetMouseDown(int button)
        {
            return IsPointerOverWorld() && Input.GetMouseButtonDown(button);
        }

        protected void CancelActions(Vector2 world)
        {
            if (Panning) Panning = false;
        }

        protected void CheckNavigation()
        {
            ZoomTo(ZoomSlider.value, (Toolbox.transform as RectTransform).anchoredPosition);

            Vector2 cursor = WCamera.ScreenToWorld(Input.mousePosition);

            // panning
            if (Panning)
            {
                Pan(cursor - pansite);
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                Panning = false;
            }
            
            if (GetMouseDown(1))
            {
                CancelActions(cursor);

                pansite = cursor;
                Panning = true;
            }

            // zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKey(KeyCode.Equals)) scroll += 5 * Time.deltaTime;
            if (Input.GetKey(KeyCode.Minus)) scroll -= 5 * Time.deltaTime;

            if (IsPointerOverWorld() && Mathf.Abs(scroll) > Mathf.Epsilon)
            {
                if (ZoomCoroutine != null) StopCoroutine(ZoomCoroutine);
                
                ZoomCoroutine = StartCoroutine(SmoothZoomTo(Zoom - scroll * 1, 0.125f));
            }
        }

        private string gistid;

        protected void CheckKeyboardShortcuts()
        {
            if (Input.GetKey(KeyCode.Alpha1)) Toolbox.PixelTab.SetSize(1);
            if (Input.GetKey(KeyCode.Alpha2)) Toolbox.PixelTab.SetSize(2);
            if (Input.GetKey(KeyCode.Alpha3)) Toolbox.PixelTab.SetSize(3);
            if (Input.GetKey(KeyCode.Alpha4)) Toolbox.PixelTab.SetSize(4);
            if (Input.GetKey(KeyCode.Alpha5)) Toolbox.PixelTab.SetSize(5);
            if (Input.GetKey(KeyCode.Alpha6)) Toolbox.PixelTab.SetSize(6);
            if (Input.GetKey(KeyCode.Alpha7)) Toolbox.PixelTab.SetSize(7);
            if (Input.GetKey(KeyCode.Alpha8)) Toolbox.PixelTab.SetSize(8);
            if (Input.GetKey(KeyCode.Alpha9)) Toolbox.PixelTab.SetSize(9);

            if (Input.GetKeyDown(KeyCode.F7))
            {
                generator.Go(project_);
            }

            if (Input.GetKeyDown(KeyCode.T) && false)
            {
                byte[] tileset = project_.tileset.texture.texture.EncodeToPNG();
                string encoding = System.Convert.ToBase64String(tileset);

                var files = new Dictionary<string, string>
                {
                    {"some file.txt", "poop"},
                    {"another file.txt", "wee"},
                    {"whatever.txt", "haha"},
                    {"tileset.png", encoding},
                };

                StartCoroutine(MakeGist.Gist.Make("testing stuffff", files, delegate(string id)
                {
                    Debug.Log(id);
                    gistid = id;
                }));
            }

            if (Input.GetKeyDown(KeyCode.U) && false)
            {
                StartCoroutine(MakeGist.Gist.Take(gistid, LoadFiles));
            }
        }

        protected void CheckHighlights()
        {
            Highlights.Highlights.SetActive(currentMode.highlights);
        }

        protected void LoadFiles(Dictionary<string, string> files)
        {
            string encoding = files["tileset.png"];

            var tileset = System.Convert.FromBase64String(encoding);

            project_.tileset.texture.texture.LoadImage(tileset);
        }

        private void UpdateHovered()
        {
            hovered.Clear();
            
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            worldRaycaster.Raycast(pointer, results);

            foreach (RaycastResult result in results)
            {
                Editable editable = result.gameObject.GetComponent<Editable>();

                if (editable != null)
                {
                    hovered.Add(editable);
                }
            }

            hovered.Add(Layer);
        }

        protected void UpdateCursors()
        {
            Vector2 cursor = WCamera.ScreenToWorld(Input.mousePosition);

            Point grid, dummy;

            Project.Grid.Coords(new Point(cursor), out grid, out dummy);
            TileCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2((grid.x + 0.5f) * Project.Grid.CellWidth,
                                                                                    (grid.y + 0.5f) * Project.Grid.CellHeight);

            Cursors.SetActive(ShowCursors);

            CheckHighlights();
        }

        private bool block;

        public Vector2 currCursorWorld;
        public Vector2 prevCursorWorld;

        protected void Update()
        {
            if (Project == null) return;

            GameObject current = EventSystem.current.currentSelectedGameObject;

            if (current && current.GetComponent<InputField>() != null)
            {
                block = true;
                return;
            }
            else if (block)
            {
                block = false;
                return;
            }

            Debug_.sprite = debug;
            Debug_.SetNativeSize();
            if (debug != null)
            {
                Debug2.anchoredPosition = debug.pivot;
            }

            prevCursorWorld = currCursorWorld;
            currCursorWorld = WCamera.ScreenToWorld(Input.mousePosition);

            CheckKeyboardShortcuts();

            UpdateHovered();

            toolIcon.sprite = null;

            currentMode.Update();

            toolIcon.transform.position = Input.mousePosition;
            toolIcon.gameObject.SetActive(toolIcon.sprite != null);

            if (Input.GetMouseButtonDown(0) && IsPointerOverWorld()) currentMode.CursorInteractStart();
            if (Input.GetMouseButtonUp(0)) currentMode.CursorInteractFinish();

            if (Input.GetKeyDown(KeyCode.Tab)) PushMode(objectMode);
            if (Input.GetKeyUp(KeyCode.Tab)) PopMode();

            if (Input.GetKeyDown(KeyCode.Tab)
             || Input.GetKeyUp(KeyCode.Tab))
            {
                CancelActions(currCursorWorld);
            }

            if (!Toolbox.gameObject.activeSelf)
            {
                CheckNavigation();
            }

            UpdateCursors();

            if (Input.GetKeyDown(KeyCode.Space)) Toolbox.Show();
            if (Input.GetKeyUp(KeyCode.Space)) Toolbox.Hide();
        }

        public void SetProject(Project project)
        {
            Project = project;

            Toolbox.SetProject(project);
        }

        public IEnumerator SmoothZoomTo(float zoom, float duration)
        {
            float start = Zoom;
            float end = zoom;
            float timer = 0;

            Vector2 focus = new Vector2(Camera.main.pixelWidth  * 0.5f,
                                        Camera.main.pixelHeight * 0.5f);

            if (start > end)
            {
                focus = Input.mousePosition;
            }

            while (timer < duration)
            {
                yield return new WaitForEndOfFrame();

                timer += Time.deltaTime;

                ZoomTo(start + timer / duration * (end - start), focus);
            }

            ZoomTo(end, focus);
        }

        public void ZoomTo(float zoom, Vector2? focus = null)
        {
            Zoom = Mathf.Clamp01(zoom);

            //if (ZoomSlider.value == Zoom) return;

            //Debug.Log(focus);

            Vector2 center = new Vector2(Camera.main.pixelWidth  * 0.5f,
                                         Camera.main.pixelHeight * 0.5f);

            Vector2 screen = focus ?? center;

            Vector2 worlda = WCamera.ScreenToWorld(screen);
            //Zoomer.localScale = (Vector3) (ZoomCurve.Evaluate(Zoom) * Vector2.one);
            WCamera.SetScale(ZoomCurve.Evaluate(Zoom));
            Vector2 worldb = WCamera.ScreenToWorld(screen);
            
            Pan(worldb - worlda);

            ZoomSlider.value = Zoom;
        }

        public void Pan(Vector2 delta)
        {
            WCamera.Pan((Vector3) (-delta));
        }

        public void MakeCharacter(Costume costume)
        {
            var character = new Character(Point.Zero, costume);

            project_.world.layers[0].characters.Add(character);
            project_.index.Add(character);

            CharacterDrawing drawing = Layer.Characters.Get(character);

            (drawing.transform as RectTransform).anchoredPosition = WCamera.ScreenToWorld(Input.mousePosition);

            StartCoroutine(Delay(delegate
            {
                Toolbox.Hide();
                objectMode.SetDrag(drawing.GetComponent<Editable>() as IObject, Vector2.zero);
            }));
        }

        public void Say(Character character, string text)
        {
            Layer.Characters.Get(character).ShowDialogue(text);
        }

        protected IEnumerator Delay(System.Action action)
        {
            yield return new WaitForEndOfFrame();

            action();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                Panning = false;
            }
        }
    }
}
