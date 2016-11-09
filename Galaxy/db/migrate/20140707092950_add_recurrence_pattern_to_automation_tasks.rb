class AddRecurrencePatternToAutomationTasks < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :recurrence_pattern, :integer
  end
end
