using System;

[Serializable]
public class SkillLevelData 
{
    public int Skill1Level;
    public int Skill2Level;
    public int Skill3Level;
    public int Skill4Level;

    public SkillLevelData(int skill1Level, int skill2Level, int skill3Level, int skill4Level)
    {
        Skill1Level = skill1Level;
        Skill2Level = skill2Level;
        Skill3Level = skill3Level;
        Skill4Level = skill4Level;
    }
    public SkillLevelData()
    {
        Skill1Level = 0;
        Skill2Level = 0;
        Skill3Level = 0;
        Skill4Level = 0;
    }
}
