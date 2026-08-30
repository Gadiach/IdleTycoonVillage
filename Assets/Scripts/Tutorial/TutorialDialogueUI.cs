using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialogueContent;
    [SerializeField] private RectTransform character;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button blackBackground;

    [Header("Character Animation")]
    [SerializeField] private float characterAnimationDuration = 0.4f;
    [SerializeField] private float characterHiddenOffset = 500f;

    [Header("Dialogue Animation")]
    [SerializeField] private float dialogueAnimationDuration = 0.25f;
    [SerializeField] private float dialogueDelay = 0.25f;

    [Header("Input")]
    [SerializeField] private float clickEnableDelay = 0.5f;

    private Vector2 characterVisiblePosition;
    private Vector2 characterHiddenPosition; 

    private Action continueAction;
    private bool canContinue;

    private Tween clickDelayTween;

    private void Awake()
    {
        characterVisiblePosition = character.anchoredPosition;

        characterHiddenPosition =
            characterVisiblePosition + new Vector2(-characterHiddenOffset, 0f);

        character.anchoredPosition = characterHiddenPosition;
        dialogueBox.localScale = Vector3.zero;

        blackBackground.onClick.AddListener(OnScreenClicked);

        dialogueContent.SetActive(false);
    }

    public void Show(string text, Action onContinue)
    {
        dialogueText.text = text;
        continueAction = onContinue;

        dialogueContent.SetActive(true);

        character.DOKill();
        dialogueBox.DOKill();

        character.anchoredPosition = characterHiddenPosition;
        dialogueBox.localScale = Vector3.zero;

        character
            .DOAnchorPos(characterVisiblePosition, characterAnimationDuration)
            .SetEase(Ease.OutCubic);

        dialogueBox
            .DOScale(Vector3.one, dialogueAnimationDuration)
            .SetDelay(dialogueDelay)
            .SetEase(Ease.OutBack);

        StartClickDelay();
    }

    public void SetDialogue(string text, Action onContinue)
    {
        dialogueText.text = text;
        continueAction = onContinue;

        dialogueBox.DOKill();

        dialogueBox.localScale = Vector3.zero;

        dialogueBox
            .DOScale(Vector3.one, dialogueAnimationDuration)
            .SetEase(Ease.OutBack);

        StartClickDelay();
    }

    public void Hide(Action onComplete = null)
    {
        canContinue = false;
        continueAction = null;

        clickDelayTween?.Kill();

        character.DOKill();
        dialogueBox.DOKill();

        dialogueBox
            .DOScale(Vector3.zero, dialogueAnimationDuration)
            .SetEase(Ease.InBack);

        character
            .DOAnchorPos(characterHiddenPosition, characterAnimationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                dialogueContent.SetActive(false);

                onComplete?.Invoke();
            });
    }

    private void StartClickDelay()
    {
        canContinue = false;

        clickDelayTween?.Kill();

        clickDelayTween = DOVirtual
            .DelayedCall(clickEnableDelay, () =>
            {
                canContinue = true;
            });
    }

    private void OnScreenClicked()
    {
        if (!canContinue)
            return;

        canContinue = false;

        continueAction?.Invoke();
    }

    private void OnDisable()
    {
        clickDelayTween?.Kill();
        character?.DOKill();
        dialogueBox?.DOKill();
    }
}