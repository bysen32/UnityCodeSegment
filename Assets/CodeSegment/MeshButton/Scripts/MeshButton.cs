using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Hoverable))]
public class MeshButton : MonoBehaviour {
    public Action<MeshButton> OnClick;

    public bool IsSelected { get; private set; }
    public string ButtonName { get { return buttonName; } }

    private enum ButtonState {
        Normal,
        Hover,
        Click,
    }

    private const float HOVER_SCALE = 1.1f;
    private const float CLICK_SCALE = 0.9f;
    private const float CLICK_DURATION = 0.2f;

    private const float SELECT_OFFSET = -.3f;
    private const float SELECT_DURATIOn = .3f;

    [SerializeField] private string buttonName;
    [SerializeField] private Transform animationTarget;

    private ButtonState state;
    private Hoverable hoverable;
    private Vector3 originalScale;
    private float scaleFactor;

    private void Awake() {
        state = ButtonState.Normal;
        hoverable = GetComponent<Hoverable>();
        scaleFactor = 1.0f;
        if (animationTarget == null) animationTarget = transform;
        originalScale = animationTarget.localScale;
    }
    
    private void Update() {
        UpdateState();
    }

    private void UpdateState() {
        if (hoverable.IsMouseOver && state == ButtonState.Normal) {
            SetState(ButtonState.Hover);
        } else if(!hoverable.IsMouseOver && state == ButtonState.Hover) {
            SetState(ButtonState.Normal);
        }

        if (Input.GetMouseButtonDown(0) && state == ButtonState.Hover) {
            SetState(ButtonState.Click);
        }

        if (state == ButtonState.Click && Input.GetMouseButtonUp(0)) {
            if (hoverable.IsMouseOver) {
                if (OnClick != null) {
                    OnClick(this);
                }
                SetState(ButtonState.Hover);
            } else {
                SetState(ButtonState.Normal);
            }
        }
    }

    private void SetState(ButtonState state) {
        this.state = state;
        StopCoroutine("AnimateClick");
        StartCoroutine("AnimateClick");
    }

    private void SetScaleFactor(float factor) {
        scaleFactor = factor;
        animationTarget.localScale = originalScale * scaleFactor;
    }

    private IEnumerator AnimateClick() {
        float startScale = scaleFactor;
        float targetScale = 1.0f;
        switch(state) {
            case ButtonState.Hover:
                targetScale = HOVER_SCALE;
                break;
            case ButtonState.Click:
                targetScale = CLICK_SCALE;
                break;
        }

        float startTime = Time.time;
        while (Time.time - startTime < CLICK_DURATION) {
            SetScaleFactor(startScale + (targetScale - startScale) * (Time.time - startTime) / CLICK_DURATION);
            yield return new WaitForEndOfFrame();
        }
        SetScaleFactor(targetScale);
    }
}
