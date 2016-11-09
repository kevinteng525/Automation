class CreateAutomationJobs < ActiveRecord::Migration
  def change
    create_table :automation_jobs do |t|
      t.text :name
      t.integer :sut_environment_id
      t.integer :test_agent_environment_id
      t.integer :job_type
      t.integer :priority
      t.integer :status
      t.integer :retry_times
      t.integer :time_out
      t.integer :create_by
      t.integer :modify_by
      t.text :description

      t.timestamps
    end
  end
end
