using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTurn : MonoBehaviour {

    private Skill_Icon m_SelectedSkill;
    private bool m_isSkillSelected;
    private Button m_Button;

    void Start()
    {
        m_SelectedSkill = null;
        m_Button = GetComponent<Button>();
        m_isSkillSelected = false;
    }


    public void SelectSkill(Skill_Icon skill)
    {
        // 아무것도 선택하지 않았을때
        if (!m_SelectedSkill && !m_isSkillSelected)
        {
            m_SelectedSkill = skill;
            NetworkManager.Instance.m_SelectedSkill = skill.m_Skill;
            m_isSkillSelected = true;
       
        }
        // 이미 스킬을 선택한 상태일때
        else if (m_SelectedSkill && m_isSkillSelected)
        {
            m_SelectedSkill.UnSelectSkill();

            m_SelectedSkill = skill;
            NetworkManager.Instance.m_SelectedSkill = skill.m_Skill;
            m_isSkillSelected = true;
          
        }

    }

    public void unSelectSkill()
    {
         m_SelectedSkill = null;
         m_isSkillSelected = false;
        
    }

    // 실제로 눌렀을 떼
    public void ClickTurn()
    {
        if (m_isSkillSelected && m_SelectedSkill)
        {
            switch (m_SelectedSkill.m_Skill)
            {
                case SKILL.DEFEND:
                    m_SelectedSkill.SkillOn();
                    unSelectSkill();

                    break;
                default:
                    if(BattleManager.Instance.IsCanClickSkill())
                    {
                        m_SelectedSkill.SkillOn();
                        unSelectSkill();
                    }
                    break;
            }
        }
        else
        {
            UIPanel_Battle.instance.SelectCaution("Select a Skill!!");
        }

    }

}
