class AddWeekDaysToAutomationTasks < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :week_days, :integer
  end
end
