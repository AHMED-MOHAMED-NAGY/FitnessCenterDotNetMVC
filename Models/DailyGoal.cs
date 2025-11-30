class DailyGoal
{
    int goalId;
    bool status; // is done or not ...
    string? goal;

    DailyGoal (string goal)
    {
        this.goal = goal;
        status = false;
    }
}