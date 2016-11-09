class AddProductIdBranchIdReleaseIdParentTaskIdToAutomationTask < ActiveRecord::Migration
  def change
    add_column :automation_tasks, :product_id, :integer
    add_column :automation_tasks, :branch_id, :integer
    add_column :automation_tasks, :release_id, :integer
    add_column :automation_tasks, :parent_task_id, :integer
  end
end
