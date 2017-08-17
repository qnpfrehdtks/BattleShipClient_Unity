using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum UI_ANIMATION
{
    NONE,
    UPPER,
    DOWNNER,
    LEFT_RIGHT,
    RIGHT_LEFT,
    UP_DOWN,
    DOWN_UP,
    FADE_IN,
    FADE_OUT,
    SHAKE
}

public class BattleInfo : MonoBehaviour {

    Text m_Text;
    Color m_OriginColor;


	// Use this for initialization
	void Start ()
    {
        m_Text = GetComponent<Text>();

        m_Text.text = " ";
        m_OriginColor = m_Text.color;

      //  SetText("DSADASD", UI_ANIMATION.FADE_IN, Color.blue);
    }

    public void SetText(string Info, UI_ANIMATION AnimationPlay, Color color, float Time = 2.0f)
    {
        if(!m_Text)
            m_Text = GetComponent<Text>();


        m_Text.color = color;
        PlayAnimation(AnimationPlay, Time);
        m_Text.text = Info;
    }

    public void reSetText(float Time)
    {
        StartCoroutine(resetTxt(Time));
    }

    private void PlayAnimation(UI_ANIMATION ani, float Time)
    {
        switch(ani)
        {
            case UI_ANIMATION.NONE:
                // 아무 것도 하지 않는다.
                break;
            case UI_ANIMATION.UPPER:
                iTween.ScaleFrom(gameObject, new Vector3(0.1f, 0.1f, 0.1f), Time);
                break;
            case UI_ANIMATION.DOWNNER:
                iTween.ScaleFrom(gameObject, new Vector3(2, 2, 2), Time);
                break;
            case UI_ANIMATION.SHAKE:
                iTween.ShakePosition(gameObject, new Vector3(10, 10, 10), Time);
                break;
            case UI_ANIMATION.LEFT_RIGHT:
                iTween.MoveFrom(gameObject, new Vector3(-1000, gameObject.transform.position.y, 0), Time);
                break;
            case UI_ANIMATION.RIGHT_LEFT:
                iTween.MoveFrom(gameObject, new Vector3(1000, gameObject.transform.position.y, 0), Time);
                break;
            case UI_ANIMATION.UP_DOWN:
                iTween.MoveFrom(gameObject, new Vector3(gameObject.transform.position.x, 1000, 0), Time);
                break;
            case UI_ANIMATION.DOWN_UP:
                iTween.MoveFrom(gameObject, new Vector3(gameObject.transform.position.x, -1000, 0), Time);
                break;
            case UI_ANIMATION.FADE_IN:
                iTween.FadeFrom(gameObject, 255, Time);
                break;
            case UI_ANIMATION.FADE_OUT:
                iTween.FadeTo(gameObject, 0.0f, Time);
                break;
        }
    }

    private IEnumerator resetTxt(float Time)
    {
        yield return new WaitForSeconds(Time);

        m_Text.text = " ";
    }



}
