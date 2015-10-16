using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using System.Linq;

using Newtonsoft.Json;
using kooltool.Data;
using Ionic.Zip;
using kooltool.Editor.Modes;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviourSingleton<Editor>
    {
        public static Sprite debug;
        public Image Debug_;
        public RectTransform Debug2;

        [SerializeField] private WorldView world;

        [Header("Camera / Canvas")]
        [SerializeField] private CameraController worldCamera;
        [SerializeField] private CameraController etherCamera;
        [SerializeField] private RectTransform viewportDivider;

        [SerializeField] private GraphicRaycaster worldRaycaster; 

        [SerializeField] private GameObject browserLayer;

        [SerializeField] private CameraController WCamera;
        [SerializeField] private Camera Camera_;
        
        [SerializeField] private Main main;

        public HighlightGroup Highlights;

        [Header("UI")]
        [SerializeField] public Tooltip tooltip;
        [SerializeField] private Toolbox toolboxPrefab;

        [Header("Cursors")]
        [SerializeField] private GameObject Cursors;
        [SerializeField] private PixelCursor PixelCursor;
        [SerializeField] private TileCursor TileCursor;
        [SerializeField] public Image toolIcon;

        [Header("Overlays")]
        [SerializeField] public ObjectOverlay objectOverlay;
        [SerializeField] private RectTransform overlaysContainer;
        [SerializeField] private GameObject noteboxEditDisable;
        [SerializeField] private InputField noteboxEditField;
        [SerializeField] private CharacterDetailsPanel characterDetails;

        private Toolbox toolbox;

        public Project project_;

        [HideInInspector] public LayerView Layer;

        public float Zoom { get; private set; }

        private Coroutine ZoomCoroutine;

        // poop
        Vector2 pansite;
        private bool Panning;
        public ProjectOld Project;

        #region Modes

        private Modes.Object objectMode;
        private Modes.Draw drawMode;
        private Modes.Tile tileMode;
        private Modes.Notes notesMode;

        private readonly Stack<Mode> modes = new Stack<Mode>();

        private Mode currentMode
        {
            get
            {
                return modes.Peek();
            }
        }

        private void PushMode(Mode mode)
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

        private void SetMode(Mode mode)
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

        public void SetNotesMode()
        {
            SetMode(notesMode);
        }

        public void SetProject(Project project)
        {
            project_ = project;

            world.SetWorld(project.world);

            Layer = world.layers.Get(project.world.layers[0]);
        }

        public void RefreshTilemap()
        {
            Layer.Tilemap.Refresh();
        }

        public Browser browser;

        protected override void Awake()
        {
            base.Awake();

            toolbox = Instantiate<Toolbox>(toolboxPrefab);
            toolbox.transform.SetParent(transform, false);
            toolbox.SetProject(this, null);
            toolbox.Hide();

            characterDetails.gameObject.SetActive(false);

            Project = new ProjectOld(new Point(32, 32));
                 
            SetProject(Data.ProjectTools.Blank());
            project_.tileset.TestTile();
            //SetProject(LoadProject("test"));

            objectMode = new Modes.Object(this);
            drawMode = new Modes.Draw(this, PixelCursor);
            tileMode = new Modes.Tile(this, TileCursor);
            notesMode = new Modes.Notes(this, PixelCursor);

            toolbox.pixelTab.SetPixelTool(drawMode);
            toolbox.tileTab.SetTileTool(tileMode);
            toolbox.notesTab.SetNotesTool(notesMode);

            modes.Push(drawMode);

            // poop
            TileCursor.mode = tileMode;
            PixelCursor.mode = drawMode;

            ZoomTo(worldCamera, 0.75f);

            SetDivide(1f);

            /*
            browser.OnConfirmed += delegate(Data.Summary summary)
            {
                browser.gameObject.SetActive(false);
                browserLayer.SetActive(false);
                SetProject(Data.ProjectTools.LoadProject(summary));
            };

            browser.Refresh();
            */

            FindObjectOfType<MapGenerator>().Go(project_, Layer.Tilemap);
        }

        public void Play()
        {
            EventSystem.current.SetSelectedGameObject(null);

            foreach (var character in Layer.Characters.Instances)
            {
                character.SetPlayer();
            }

            toolbox.Hide();

            main.SetPlayer(project_);
        }

        public void Save()
        {
            StartCoroutine(project_.index.SaveCO(project_));

            var summary = new Data.Summary
            {
                title = project_.index.folder,
                description = "unset",
                icon = "icon.png",
                iconSprite = PixelDraw.Brush.Circle(128, new Color(Random.value, Random.value, Random.value, 1)),
                folder = project_.index.folder,
                root = project_.index.root,
            };

            Data.ProjectTools.SaveSummary(summary);
        }

#if UNITY_EDITOR
        private static string exepath = "../kooltool/";
#else
        private static string exepath = "../..";
#endif
        public void Export()
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

        private void CheckNavigation()
        {
            Vector2 cursor = WCamera.ScreenToWorld(Input.mousePosition);

            // panning
            if (Panning)
            {
                // camera focus should situate pansite underneath cursor
                WCamera.focus -= (cursor - pansite);
                WCamera.Halt();
            }
            else
            {
                float dt = Time.deltaTime;

                if (Input.GetKey(KeyCode.LeftShift)
                 || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKey(KeyCode.UpArrow))    WCamera.scaleTarget += WCamera.scaleTarget * dt;
                    if (Input.GetKey(KeyCode.DownArrow))  WCamera.scaleTarget -= WCamera.scaleTarget * dt;
                    if (Input.GetKey(KeyCode.LeftArrow))  WCamera.rotationTarget += 90 * dt;
                    if (Input.GetKey(KeyCode.RightArrow)) WCamera.rotationTarget -= 90 * dt;
                }
                else
                {
                    float scale = 1000 / WCamera.scale * dt;

                    if (Input.GetKey(KeyCode.UpArrow))    WCamera.focusTarget += WCamera.up    * scale;
                    if (Input.GetKey(KeyCode.DownArrow))  WCamera.focusTarget -= WCamera.up    * scale;
                    if (Input.GetKey(KeyCode.LeftArrow))  WCamera.focusTarget -= WCamera.right * scale;
                    if (Input.GetKey(KeyCode.RightArrow)) WCamera.focusTarget += WCamera.right * scale;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                Panning = false;
            }
            
            if (IsPointerOverWorld() && Input.GetMouseButtonDown(1))
            {
                pansite = cursor;
                Panning = true;
            }

            // zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKey(KeyCode.Equals)) scroll += 5 * Time.deltaTime;
            if (Input.GetKey(KeyCode.Minus)) scroll -= 5 * Time.deltaTime;

            if (Mathf.Abs(scroll) > Mathf.Epsilon)
            {
                if (ZoomCoroutine != null) StopCoroutine(ZoomCoroutine);

                if (etherCamera.focussed)
                {
                    ZoomCoroutine = StartCoroutine(SmoothZoomTo(Zoom - scroll * 1, 0.125f, etherCamera));
                }
                else if (IsPointerOverWorld())
                {
                    ZoomCoroutine = StartCoroutine(SmoothZoomTo(Zoom - scroll * 1, 0.125f, worldCamera));
                }
            }
        }

        private string gistid;

        private void CheckKeyboardShortcuts()
        {
            if (Input.GetKey(KeyCode.Alpha1)) toolbox.pixelTab.SetSize(1);
            if (Input.GetKey(KeyCode.Alpha2)) toolbox.pixelTab.SetSize(2);
            if (Input.GetKey(KeyCode.Alpha3)) toolbox.pixelTab.SetSize(3);
            if (Input.GetKey(KeyCode.Alpha4)) toolbox.pixelTab.SetSize(4);
            if (Input.GetKey(KeyCode.Alpha5)) toolbox.pixelTab.SetSize(5);
            if (Input.GetKey(KeyCode.Alpha6)) toolbox.pixelTab.SetSize(6);
            if (Input.GetKey(KeyCode.Alpha7)) toolbox.pixelTab.SetSize(7);
            if (Input.GetKey(KeyCode.Alpha8)) toolbox.pixelTab.SetSize(8);
            if (Input.GetKey(KeyCode.Alpha9)) toolbox.pixelTab.SetSize(9);

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

        private void CheckHighlights()
        {
            Highlights.Highlights.SetActive(currentMode.highlights);
        }

        private void LoadFiles(Dictionary<string, string> files)
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

            hovered.AddRange(results.Select(result => result.gameObject.GetComponent<Editable>())
                                    .Where(editable => editable != null));

            hovered.Add(Layer);
        }

        private void UpdateCursors()
        {
            Vector2 cursor = WCamera.ScreenToWorld(Input.mousePosition);

            Point grid, dummy;

            Project.Grid.Coords(new Point(cursor), out grid, out dummy);
            TileCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2((grid.x + 0.5f) * Project.Grid.CellWidth,
                                                                                    (grid.y + 0.5f) * Project.Grid.CellHeight);

            Cursors.SetActive(ShowCursors);
            toolIcon.enabled = toolIcon.sprite != null && ShowCursors;

            CheckHighlights();
        }

        public void UpdateCameras()
        {
            float u = (1 + Mathf.Sin(Time.timeSinceLevelLoad)) * 0.5f;

            u = Input.mousePosition.x / Screen.width;

            SetDivide(u);
        }

        private float divide = .99f;

        private void SetDivide(float u)
        {
            u = Mathf.Clamp(u, 0.01f, 0.99f);

            divide = u;

            float l = u;
            float r = 1 - u;

            worldCamera.viewport = new UnityEngine.Rect(0, 0, l, 1);
            etherCamera.viewport = new UnityEngine.Rect(l, 0, r, 1);
        }

        private bool block;

        public Vector2 currCursorWorld;
        public Vector2 prevCursorWorld;

        private float tilt;
        private float tiltVelocity;

        private float sep;
        private float sepVelocity;

        private void Update()
        {
            if (Project == null) return;

            var ptrans = viewportDivider.parent as RectTransform;
            viewportDivider.anchoredPosition = Vector2.right * divide * ptrans.rect.width;

            //UpdateCameras();

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

            if (Input.GetKeyDown(KeyCode.Tab)) SetMode(objectMode);
            //if (Input.GetKeyUp(KeyCode.Tab)) PopMode();

            if (Input.GetKeyDown(KeyCode.Tab)
             || Input.GetKeyUp(KeyCode.Tab))
            {
                Panning = false;
            }

            if (!toolbox.gameObject.activeSelf)
            {
                CheckNavigation();
            }

            UpdateCursors();

            if (Input.GetKeyDown(KeyCode.Space)) toolbox.Show();
            if (Input.GetKeyUp(KeyCode.Space)) toolbox.Hide();

            bool split = Input.GetKey(KeyCode.V);
            tilt = Mathf.SmoothDamp(tilt, split ? 1 : 0, ref tiltVelocity, .1f);
            sep = Mathf.SmoothDamp(sep, (split & tilt >= 0.5f) ? 50 : 0, ref sepVelocity, .3f);

            WCamera.pivotTarget = tilt * 45;

            foreach (var layer in world.layers.Instances)
            {
                layer.separation = sep;
            }
        }

        public IEnumerator SmoothZoomTo(float zoom, 
                                        float duration, 
                                        CameraController camera)
        {
            float start = Zoom;
            float end = zoom;
            float timer = 0;

            Vector2 focus = camera.pixelRect.center;

            if (start > end)
            {
                focus = Input.mousePosition;
            }

            while (timer < duration)
            {
                yield return new WaitForEndOfFrame();

                timer += Time.deltaTime;

                ZoomTo(camera, start + timer / duration * (end - start), focus);
            }

            ZoomTo(camera, end, focus);
        }

        public void ZoomTo(CameraController camera,
                           float zoom, 
                           Vector2? focus = null)
        {
            Zoom = Mathf.Clamp01(zoom);

            //Debug.Log(focus);

            Vector2 center = new Vector2(Camera.main.pixelWidth  * 0.5f,
                                         Camera.main.pixelHeight * 0.5f);

            Vector2 screen = focus ?? center;

            Vector2 worlda = camera.ScreenToWorld(screen);
            //Zoomer.localScale = (Vector3) (ZoomCurve.Evaluate(Zoom) * Vector2.one);
            camera.scale = UISettings.Instance.navigation.zoomCurve.Evaluate(Zoom);
            Vector2 worldb = camera.ScreenToWorld(screen);

            camera.focus -= (worldb - worlda);
            camera.Halt();
        }

        public void MakeNotebox(string text)
        {
            var notebox = new Notebox
            {
                position = Point.Zero,
                text = text,
            };

            project_.world.layers[0].noteboxes.Add(notebox);

            NoteboxView view = Layer.noteboxes.Get(notebox);

            (view.transform as RectTransform).anchoredPosition = WCamera.ScreenToWorld(Input.mousePosition);

            StartCoroutine(Delay(delegate
            {
                toolbox.Hide();
                objectMode.SetDrag(view, Vector2.zero);
            }));
        }
        
        public void RemoveNotebox(Notebox notebox)
        {
            project_.world.layers[0].noteboxes.Remove(notebox);

            Layer.noteboxes.Discard(notebox);

            objectOverlay.SetSubject(null);
        }

        private Notebox nbediting;
        public void EditNotebox(Notebox notebox)
        {
            nbediting = notebox;

            noteboxEditDisable.SetActive(true);
            noteboxEditField.text = notebox.text;
            noteboxEditField.onEndEdit.RemoveAllListeners();
            noteboxEditField.Select();
            noteboxEditField.onEndEdit.AddListener(EndEditNB);
        }

        private void EndEditNB(string text)
        {
            nbediting.text = text;
            Layer.noteboxes.Get(nbediting).Refresh();
            nbediting = null;
            noteboxEditDisable.SetActive(false);
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
                toolbox.Hide();
                objectMode.SetDrag(drawing.GetComponent<Editable>() as IObject, Vector2.zero);
            }));
        }

        public void RemoveCharacter(Character character)
        {
            project_.world.layers[0].characters.Remove(character);
            project_.index.Remove(character);

            Layer.Characters.Discard(character);

            objectOverlay.SetSubject(null);
        }

        public void EditScript(Character character)
        {
            if (characterDetails.gameObject.activeSelf
             && characterDetails.character == character)
            {
                characterDetails.gameObject.SetActive(false);
            }
            else
            {
                characterDetails.gameObject.SetActive(true);
                characterDetails.SetCharacter(character);
            }
        }

        private IEnumerator Delay(System.Action action)
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
