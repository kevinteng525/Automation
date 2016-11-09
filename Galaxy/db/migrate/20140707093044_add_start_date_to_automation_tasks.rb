class AddStartDateToAutomationTasks < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :start_date, :datetime
  end
end
