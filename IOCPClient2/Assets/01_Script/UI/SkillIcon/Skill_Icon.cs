using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SKILL
{
    BASE,
    DEFEND,
    RADER,
    SUPER_BOMB
    
};

public class Skill_Icon : MonoBehaviour {

    public PlayTurn m_PlaySkill;

    public GameObject m_Info;
   public int m_Range;
   public ATTKSHAPE m_Shape;
   public SKILL m_Skill;

   public Button m_Button;
   public Image m_SkillX;
   public Text m_SkillNumTXT;

   private int m_SkillCurNum;
   public int m_SkillMaxNum;

    public bool m_isSelected { get; private set; }

    void Start()
    {
        m_isSelected = false;
        m_Info.SetActive(false);
           m_SkillCurNum = m_SkillMaxNum;
        m_SkillX.gameObject.SetActive(false);

        if (m_Skill != SKILL.BASE)
            m_SkillNumTXT.text = m_SkillCurNum.ToString() + "/" + m_SkillMaxNum.ToString();
        else m_SkillNumTXT.text = "∞";
    }
    

    public void ClickSkill()
    {
        ////if(!BattleManager.Instance.IsCanClickSkill())
        ////{
        ////    return;
        ////}


        if (m_SkillCurNum > 0 )
        {
            SoundManager.Instance.playSoundOnseShot("ClickSkill");
            if (m_isSelected)
            {
                UnSelectSkill();
                m_PlaySkill.unSelectSkill();
            }
            else
            {
                SelectSkill();
            }
        }

    }


    public void SelectSkill()
    { 
            m_isSelected = true;
            m_Button.image.color = Color.grey;
            m_Info.SetActive(true);
            m_PlaySkill.SelectSkill(this);
           
    }

    public void UnSelectSkill()
    {
            m_isSelected = false;
            m_Button.image.color = Color.white;
            m_Info.SetActive(false);
           // m_PlaySkill.unSelectSkill();
    }



    public void SkillOn()
    {
        BattleManager.Instance.AttackBlock(m_Range, m_Shape, m_Skill);
        UpdateSkillState();
        UnSelectSkill();
    }

    private void UpdateSkillState()
    {
       if(m_Skill != SKILL.BASE)
       { 
        m_SkillCurNum--;
        m_SkillNumTXT.text = m_SkillCurNum.ToString() + "/" + m_SkillMaxNum.ToString();

            if (m_SkillCurNum <= 0)
            {
                m_SkillNumTXT.color = Color.red;

                m_SkillX.gameObject.SetActive(true);

                m_Button.image.color = Color.grey;
                m_Button.transition = Selectable.Transition.None;
            }
        }
    }
}
