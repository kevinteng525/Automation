class AddWeekIntervalToAutomationTasks < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :week_interval, :integer
  end
end
