class AddInfoToTestCaseExecution < ActiveRecord::Migration
  def change
    add_column :test_case_executions, :info, :text
  end
end
