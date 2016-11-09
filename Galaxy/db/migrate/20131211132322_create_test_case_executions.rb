class CreateTestCaseExecutions < ActiveRecord::Migration
  def change
    create_table :test_case_executions do |t|
      t.integer :test_case_id
      t.integer :job_id
      t.integer :status
      t.datetime :start_time
      t.datetime :end_time
      t.integer :retry_times

      t.timestamps
    end
  end
end
