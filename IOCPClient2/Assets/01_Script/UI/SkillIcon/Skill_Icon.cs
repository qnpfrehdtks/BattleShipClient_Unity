using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SKILL
{
    BASE,
    REPAIR,
    RADER,
    SUPER_BOMB
    
};

public class Skill_Icon : MonoBehaviour {

   public int m_Range;
   public ATTKSHAPE m_Shape;
   public SKILL m_Skill;

   public Button m_Button;
   public Image m_SkillX;
   public Text m_SkillNumTXT;

   private int m_SkillCurNum;
   public int m_SkillMaxNum;

    void Start()
    {
        m_SkillCurNum = m_SkillMaxNum;
        m_SkillX.gameObject.SetActive(false);

        if (m_Skill != SKILL.BASE)
            m_SkillNumTXT.text = m_SkillCurNum.ToString() + "/" + m_SkillMaxNum.ToString();
        else m_SkillNumTXT.text = "∞";
    }
    
    public void ClickSkill()
    {
        if(m_SkillCurNum > 0)
        {
            UpdateSkillState();
            NetworkManager.Instance.m_SelectedSkill = m_Skill;
            BattleManager.Instance.ClickSkill(this);
      
        }
    }

    public void SkillOn()
    {
        BattleManager.Instance.AttackBlock(m_Range, m_Shape, m_Skill);
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
