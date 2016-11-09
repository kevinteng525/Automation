require 'test_helper'

class AutomationTasksControllerTest < ActionController::TestCase
  setup do
    @automation_task = automation_tasks(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:automation_tasks)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create automation_task" do
    assert_difference('AutomationTask.count') do
      post :create, automation_task: { build_id: @automation_task.build_id, create_by: @automation_task.create_by, create_date: @automation_task.create_date, description: @automation_task.description, information: @automation_task.information, modify_by: @automation_task.modify_by, modify_date: @automation_task.modify_date, name: @automation_task.name, priority: @automation_task.priority, status: @automation_task.status, supported_environment_id: @automation_task.supported_environment_id, task_type: @automation_task.task_type, test_content: @automation_task.test_content }
    end

    assert_redirected_to automation_task_path(assigns(:automation_task))
  end

  test "should show automation_task" do
    get :show, id: @automation_task
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @automation_task
    assert_response :success
  end

  test "should update automation_task" do
    patch :update, id: @automation_task, automation_task: { build_id: @automation_task.build_id, create_by: @automation_task.create_by, create_date: @automation_task.create_date, description: @automation_task.description, information: @automation_task.information, modify_by: @automation_task.modify_by, modify_date: @automation_task.modify_date, name: @automation_task.name, priority: @automation_task.priority, status: @automation_task.status, supported_environment_id: @automation_task.supported_environment_id, task_type: @automation_task.task_type, test_content: @automation_task.test_content }
    assert_redirected_to automation_task_path(assigns(:automation_task))
  end

  test "should destroy automation_task" do
    assert_difference('AutomationTask.count', -1) do
      delete :destroy, id: @automation_task
    end

    assert_redirected_to automation_tasks_path
  end
end
