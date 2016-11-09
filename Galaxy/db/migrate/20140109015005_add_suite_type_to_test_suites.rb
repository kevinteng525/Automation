class AddSuiteTypeToTestSuites < ActiveRecord::Migration
  def change
    add_column :test_suites, :suite_type, :integer
  end
end
