class ChangeStartTimeStartDateToStringInAutomationTask < ActiveRecord::Migration
  def change
    change_column :automation_tasks, :start_time, :text
    change_column :automation_tasks, :start_date, :text
  end
end
