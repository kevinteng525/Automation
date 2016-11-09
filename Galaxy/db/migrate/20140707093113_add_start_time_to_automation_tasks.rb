class AddStartTimeToAutomationTasks < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :start_time, :datetime
  end
end
