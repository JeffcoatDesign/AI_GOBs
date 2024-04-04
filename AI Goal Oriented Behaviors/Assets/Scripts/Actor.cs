using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    List<Goal> myGoals;
    List<Action> myActions;
    Action decayAction;
    private void Awake()
    {
        myGoals = new List<Goal>
        {
            new Goal("Eat", 5f),
            new Goal("Drink", 4f),
            new Goal("Sleep", 4f),
            new Goal("Bathroom", 4f)
        };

        myActions = new List<Action>
        {
            new Action("Eat a burger")
            {
                effectedGoals = new List<Goal>
                {
                    new Goal("Eat", -4f),
                    new Goal("Drink", 2f),
                    new Goal("Sleep", 1f),
                    new Goal("Bathroom", 2f)
                }
            },
            new Action("Eat a cookie")
            {
                effectedGoals = new List<Goal>
                {
                    new Goal("Eat", -2f),
                    new Goal("Drink", 1f),
                    new Goal("Sleep", -1f),
                    new Goal("Bathroom", 1f)
                }
            },
            new Action("Drink water")
            {
                effectedGoals = new List<Goal>
                {
                    new Goal("Eat", -1f),
                    new Goal("Drink", -4f),
                    new Goal("Sleep", -1f),
                    new Goal("Bathroom", 2f)
                }
            },
            new Action("Go to sleep")
            {
                effectedGoals = new List<Goal>
                {
                    new Goal("Eat", 2f),
                    new Goal("Drink", 1f),
                    new Goal("Sleep", -4f),
                    new Goal("Bathroom", 1f)
                }
            },
            new Action("Go to the bathroom")
            {
                effectedGoals = new List<Goal>
                {
                    new Goal("Eat", 0f),
                    new Goal("Drink", 0f),
                    new Goal("Sleep", 0f),
                    new Goal("Bathroom", -5f)
                }
            }
        };

        decayAction = new Action("Decay");
        decayAction.effectedGoals = new List<Goal>
        {
            new Goal("Eat", 2f),
            new Goal("Drink", 2f),
            new Goal("Sleep", 2f),
            new Goal("Bathroom", 2f)
        };

        InvokeRepeating(nameof(Decay), 0f, 5f);
        InvokeRepeating(nameof(TakeAction), 1f, 10f);
    }

    private void Decay()
    {
        string goalsStatus = "";
        foreach (Goal goal in myGoals)
        {
            goalsStatus += goal.name + " " + goal.value + "\n";
            goal.value += decayAction.GetGoalChange(goal);
        }
        textMeshProUGUI.text = goalsStatus;
        Debug.Log(goalsStatus);
    }
    private void TakeAction () {
        Action action = ChooseAction(myActions, myGoals);
        string goalsStatus = "";
        foreach (Goal goal in myGoals)
        {
            goal.value += action.GetGoalChange(goal);
            goalsStatus += goal.name + " " + goal.value + "\n";
        }
        textMeshProUGUI.text = goalsStatus + "I will: " + action.name;
        Debug.Log("I will: " + action.name + "\n" + goalsStatus);
    }

    Action ChooseAction (List<Action> actions, List<Goal> goals)
    {
        Action bestAction = actions[0];
        float bestValue = float.MaxValue;

        foreach (var action in actions) {
            float thisValue = GetDisconentment(action, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    private float GetDisconentment(Action action, List<Goal> goals)
    {
        float discontentment = 0f;

        foreach (var goal in goals)
        {
            float newValue = goal.value + action.GetGoalChange(goal);

            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }
}

public class Action {
    public string name;
    public List<Goal> effectedGoals;

    public Action (string name)
    {
        this.name = name;
    }

    public float GetGoalChange(Goal goal)
    {
        Goal effectedGoal = effectedGoals.FirstOrDefault(g => g.name == goal.name);
        if (effectedGoal != null) return effectedGoal.value;
        return 0f;
    }
}
public class Goal {
    public string name;
    public float value;

    public Goal (string name, float value)
    {
        this.name = name;
        this.value = value;
    }

    public float GetDiscontentment(float newValue)
    {
        return newValue * newValue;
    }
}