class CreateAutomationTaskRunningStatuses < ActiveRecord::Migration
  def change
    create_table :automation_task_running_statuses do |t|
      t.integer :task_id
      t.integer :status
      t.text :information
      t.integer :execution_percentage
      t.string :result_type_list
      t.string :result_count_list

      t.timestamps
    end
  end
end
