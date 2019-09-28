using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class AutoMinSize : UIBehaviour, ILayoutElement
{
    [SerializeField]
    protected RectTransform maxWidthProvider;
    [SerializeField]
    protected RectTransform maxHeightProvider;

    public RectTransform MaxWidthProvider
    {
        get => this.maxWidthProvider;
        set
        {
            if (this.maxWidthProvider != value)
            {
                this.maxWidthProvider = value;
                this.SetDirty();
            }
        }
    }

    public RectTransform MaxHeightProvider
    {
        get { return this.maxHeightProvider; }
        set
        {
            if (this.maxHeightProvider != value)
            {
                this.maxHeightProvider = value;
                this.SetDirty();
            }
        }
    }

    public virtual float minWidth
    {
        get
        {
            if (this.MaxWidthProvider == null)
                return -1;

            // Anti-StackOverflowException aka Unity-Retardation-Protector (tm) (patent pending).
            RectTransform temp = this.MaxWidthProvider;
            this.maxWidthProvider = null;
            float result = Mathf.Min(temp.rect.width, LayoutUtility.GetPreferredWidth(this.RectTransform));
            this.maxWidthProvider = temp;
            return result;
        }
    }

    public virtual float minHeight
    {
        get
        {
            if (this.MaxHeightProvider == null)
                return -1;

            // Anti-StackOverflowException aka Unity-Retardation-Protector (tm) (patent pending).
            RectTransform temp = this.MaxHeightProvider;
            this.maxHeightProvider = null;
            float result = Mathf.Min(temp.rect.height, LayoutUtility.GetPreferredHeight(this.RectTransform));
            this.maxHeightProvider = temp;
            return result;
        }
    }

    public virtual float preferredWidth { get => -1; }
    public virtual float preferredHeight { get => -1; }
    public virtual float flexibleWidth { get => -1; }
    public virtual float flexibleHeight { get => -1; }
    public virtual int layoutPriority { get => 1; }

    private RectTransform _rectTransform;
    private RectTransform RectTransform
    {
        get
        {
            this._rectTransform = this._rectTransform ?? this.GetComponent<RectTransform>();
            return this._rectTransform;
        }
    }

    protected AutoMinSize()
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.SetDirty();
    }

    protected override void OnTransformParentChanged()
    {
        this.SetDirty();
    }

    protected override void OnDisable()
    {
        this.SetDirty();
        base.OnDisable();
    }

    protected override void OnDidApplyAnimationProperties()
    {
        this.SetDirty();
    }

    protected override void OnBeforeTransformParentChanged()
    {
        this.SetDirty();
    }

    public virtual void CalculateLayoutInputHorizontal()
    {
    }

    public virtual void CalculateLayoutInputVertical()
    {
    }

    protected void SetDirty()
    {
        if (!this.IsActive())
            return;
        LayoutRebuilder.MarkLayoutForRebuild(this.RectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        this.SetDirty();
    }
#endif
}