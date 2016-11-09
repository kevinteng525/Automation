class AddFilesToTestResults < ActiveRecord::Migration
  def change
    add_column :test_results, :files, :string
  end
end
