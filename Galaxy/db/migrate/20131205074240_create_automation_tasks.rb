class CreateAutomationTasks < ActiveRecord::Migration
  def change
    create_table :automation_tasks do |t|
      t.string :name
      t.integer :status
      t.integer :task_type
      t.integer :priority
      t.datetime :create_date
      t.integer :create_by
      t.datetime :modify_date
      t.integer :modify_by
      t.integer :build_id
      t.integer :supported_environment_id
      t.string :test_content
      t.text :information
      t.text :description

      t.timestamps
    end
  end
end
